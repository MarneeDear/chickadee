namespace chickadee.infrastructure.DireWolf

open chickadee.core
open chickadee.core.TNC2MonActivePatterns
open chickadee.core.DataFormats.DataFormatType
open chickadee.infrastructure.database
open chickadee.core.PositionReport
open System
open System.IO
open FSharp.Control.Tasks
open System.Threading.Tasks
open Microsoft.Data.Sqlite

module KissUtil =
    open chickadee.infrastructure.TNC2MONRepository

    let private timestamp = (DateTime.Now.ToString("yyyyMMddHHmmssff"))

    let private kiss commands =
        commands
        |> Option.defaultValue []
        |> List.map (fun c -> string (c.ToString()))   
        
    let private file path = Path.Combine(Path.GetFullPath(path), sprintf "%s%s" timestamp "chick.txt")

    //All received frames are displayed in the usual monitor format, preceded with the channel number inside of [ ].
    //[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
    //See Dire Wolf User Guide 14.6 kissutil – KISS TNC troubleshooting and Application Interface
    let processKissUtilFramesInDirectory path (file: string option) =
        let d = new DirectoryInfo(path) //Assuming Test is your Folder
        let files = d.GetFiles()  //GetFiles("*.txt"); //Getting Text files
        let getFrames fileName = 
            File.ReadAllLines (Path.Combine(path, fileName))
            |> Array.map (fun f -> convertRecordToAPRSData f)
        match file with
        | Some f    -> getFrames f
        | None      -> files 
                        |> Array.map (fun f -> f.Name)
                        |> Array.map getFrames
                        |> Array.head
 
    let processKissUtilFramesInFile (file: string) =
        File.ReadAllLines file 
        |> Array.map (fun f -> convertRecordToAPRSData f)

    [<CLIMutable>]
    type RawReceivedFrame =
        {
            date_created : string
            raw_packet   : string
            packet_type  : string
            error        : string
        }

    [<CLIMutable>]
    type RawTransmittedFrame =
        {
            rowid : int
            date_created : string
            raw_packet : string
            packet_type : string
            transmitted : int
        }

    type TransmittedList =
        {
            list : string
        }

    type TransmittedId =
        {
            id : int
        }

    let saveTransmitFrame connectionString (frame:RawTransmittedFrame) : Task<Result<int,exn>> =
        task {
            use connection = new SqliteConnection(connectionString)
            return! execute connection "INSERT into transmitted (raw_packet, packet_type)
                                        VALUES (@raw_packet, @packet_type);" frame
        }

    let savePacketToDatabase (connectionString:string) (p:TNC2MON.Packet) =
        let pType =
            match p.Information with
            | Some i -> match i with
                        | TNC2MON.Message _ -> APRSDataFormats.Message.ToString()
                        | TNC2MON.PositionReport prt -> match prt with
                                                        | PositionReportFormat.PositionReportWithoutTimeStampOrUltimeter p -> APRSDataFormats.PositionReportWithoutTimeStampOrUltimeter.ToString()
                                                        | PositionReportFormat.PositionReportWithoutTimeStampWithMessaging p -> APRSDataFormats.PositionReportWithoutTimeStampWithMessaging.ToString()
                                                        | PositionReportFormat.PositionReportWithTimestampNoMessaging p -> APRSDataFormats.PositionReportWithTimestampNoMessaging.ToString()
                                                        | PositionReportFormat.PositionReportWithTimestampWithMessaging p -> APRSDataFormats.PositionReportWithTimestampWithMessaging.ToString()
            | None -> String.Empty
        
        let frame =
            {
                rowid = 0
                date_created = String.Empty
                raw_packet = p.ToString()
                packet_type = pType
                transmitted = 0
            }        
        saveTransmitFrame connectionString frame

    let getTransmitFrames connectionString (transmitted:bool option) (packetType:string option) : Task<Result<RawTransmittedFrame seq, exn>> =
        task {
            use connection = new SqliteConnection(connectionString)
            match transmitted, packetType with
            | Some t, Some p -> return! query connection "SELECT rowid, date_created, raw_packet, packet_type, transmitted 
                                                          FROM transmitted
                                                          WHERE transmitted = @trans and packet_type = @packetType;" (Some <| dict ["trans" => t; "packetType" => p])            
            | Some t, None -> return! query connection "SELECT rowid, date_created, raw_packet, packet_type, transmitted 
                                                        FROM transmitted
                                                        WHERE transmitted = @trans;" (Some <| dict ["trans" => t])
            | None, Some p -> return! query connection "SELECT rowid, date_created, raw_packet, packet_type, transmitted 
                                                        FROM transmitted
                                                        WHERE packet_type = @packetType;" (Some <| dict ["packetType" => p])            
            | None, None -> return! query connection "SELECT rowid, date_created, raw_packet, packet_type, transmitted 
                                                      FROM transmitted;" None
                                                      
        }

    let setTransmitted connectionString (idList: int list) : Task<Result<int,exn>> =
        let update list =
            sprintf "UPDATE transmitted SET transmitted = 1 WHERE rowid IN (%s);" list
        let execute (idList:string) =            
            task {
                use connection = new SqliteConnection(connectionString)
                return! execute connection (update idList) None
            }
        idList |> List.map string |> String.concat "," |> execute

    let writeFramesToKissUtil (commands: KISS.Command list option) (saveTo:string) (frames:string list) =
        File.WriteAllLines (file saveTo, (kiss commands) @ frames) |> ignore //put the commands first and then the frames
        file saveTo

    let writeStoredFramesToKissUtil (connectionString:String) (commands: KISS.Command list option) (path:string) =
        let doGet =
            task {
                return! getTransmitFrames connectionString None None
            }
        match doGet.Result with
        | Ok result when result |> Seq.length > 0 -> 
                                                    ((result |> Seq.map (fun r -> r.raw_packet) 
                                                    |> Seq.toList |> writeFramesToKissUtil commands path) , (result |> Seq.map (fun r -> r.rowid))) 
                                                    |> Ok                      
        | Ok _ -> (String.Empty, Seq.empty) |> Ok
        | Error exn -> Error exn

    (*
    Files are deleted after they are transmitted
    Write a TNC2MON packet to a file that will be read and transmitted by Dire Wolf vis the kissutil
    See Dire Wolf User Guide section 14.6.3 Transmit frames from files
    *)
    let writePacketsToKissUtil (commands: KISS.Command list option) (saveTo:string) (packets: TNC2MON.Packet list) =       
        let frames = 
            packets 
            |> List.map (fun p -> p.ToString())
        
        File.WriteAllLines (file saveTo, (kiss commands) @ frames) |> ignore //put the commands first and then the frames
        file saveTo

    let saveRawReceivedFrame connectionString (frame:RawReceivedFrame) : Task<Result<int,exn>> =
        task { 
            use connection = new SqliteConnection(connectionString)
            return! execute connection "INSERT INTO received(raw_packet, packet_type, error)
            VALUES (@raw_packet, @packet_type, @error);" frame
        }

    let private informationType frame =
        match (|Information|_|) frame with
        | Some i    ->  match (|FormatType|_|) i with
                        | Some t -> t.ToString() |> Ok
                        | None -> APRSDataFormats.DataFormat.Unsupported.ToString() |> Ok                    
        | None      -> "No information part found." |> Error

    let private getRecords path (file: string option) =
            let d = new DirectoryInfo(path);//Assuming Test is your Folder
            let files = d.GetFiles()  
            let getRecords fileName = File.ReadAllLines (Path.Combine(path, fileName))
            match file with
            | Some f -> getRecords f
            | None   -> files |> Array.map (fun f -> f.Name) |> Array.map (fun f -> getRecords f) |> Array.head
        
    let saveReceivedRawRecords connectionString path (file: string option) =
        let rcrds = 
            match file with
            | None -> getRecords path None
            | Some f -> getRecords path (Some f)

        let mapToRawFrameRecord error (frame:string) (packet_type:string)  =
            {
                date_created = String.Empty
                raw_packet = frame
                packet_type = packet_type 
                error = error
            }
        let doSave frame =
            task {
                System.Threading.Thread.Sleep(1000) //Need to wait so the records have a new created date which is the primary key
                return! (saveRawReceivedFrame connectionString frame)
            }           
        let saveFrame rcrd = 
            match (|Frame|_|) rcrd with
            | Some f -> match informationType f with
                        | Ok t -> mapToRawFrameRecord String.Empty f t |> (fun p -> (doSave p).Result)
                        | Error msg -> mapToRawFrameRecord msg f msg |> (fun f -> (doSave f).Result)
            | None   -> mapToRawFrameRecord rcrd "Record not in expected format." "Record not in expected format." |> (fun f -> (doSave f).Result)
 
        rcrds |> Array.map saveFrame

    let getReceivedFrames connectionString (packetType:string option) : Task<Result<RawReceivedFrame seq, exn>> =
        task {
            use connection = new SqliteConnection(connectionString)
            match packetType with
            | Some t -> return! query connection "SELECT date_created, raw_packet, packet_type
                                                  FROM received WHERE packet_type = @packetType;" (Some <| dict ["packetType" => t])
            | None -> return! query connection "SELECT date_created, raw_packet, packet_type
                                                FROM received;" None            
        }

