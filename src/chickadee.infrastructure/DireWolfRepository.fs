namespace chickadee.infrastructure.DireWolf

open chickadee.core
open chickadee.core.TNC2MonActivePatterns
open chickadee.infrastructure.database
open System
open System.IO
open FSharp.Control.Tasks
open System.Threading.Tasks
open Microsoft.Data.Sqlite
open chickadee.core.TNC2MON
open System.Collections.Generic

module KissUtil =
    open chickadee.infrastructure.TNC2MONRepository

    let timestamp = (DateTime.Now.ToString("yyyyMMddHHmmssff"))

    let kiss commands =
        commands
        |> Option.defaultValue []
        |> List.map (fun c -> string (c.ToString()))      

    (*
    Files are deleted after they are transmitted
    Write a TNC2MON packet to a file that will be read and transmitted by Dire Wolf vis the kissutil
    See Dire Wolf User Guide section 14.6.3 Transmit frames from files
    *)
    let writePacketToKissUtil (commands: KISS.Command list option) (saveTo:string) (packets: TNC2MON.Packet list) =
        let file = Path.Combine(Path.GetFullPath(saveTo), sprintf "%s%s" timestamp "chick.txt")        
        
        let frames = 
            packets 
            |> List.map (fun p -> p.ToString())
        
        File.WriteAllLines (file, (kiss commands) @ frames) |> ignore //put the commands first and then the frames
        file

    let writeFrameToKissUtil (commands: KISS.Command list option) (saveTo:string) (frames:string list) =
        let file = Path.Combine(Path.GetFullPath(saveTo), sprintf "%s%s" timestamp "chick.txt")
        File.WriteAllLines (file, (kiss commands) @ frames) |> ignore //put the commands first and then the frames
        file

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
         //let d = new DirectoryInfo(path) //Assuming Test is your Folder
         //let files = d.GetFiles()  //GetFiles("*.txt"); //Getting Text files
        File.ReadAllLines file // (Path.Combine(path, fileName))
        |> Array.map (fun f -> convertRecordToAPRSData f)

    [<CLIMutable>]
    type RawReceivedFrameRecord =
        {
            raw_packet   : string
            packet_type  : string
            error        : string
        }

    [<CLIMutable>]
    type RawTransmitted =
        {
            rowid : int
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

    let saveTransmitFrame connectionString (frame:RawTransmitted) : Task<Result<int,exn>> =
        task {
            use connection = new SqliteConnection(connectionString)
            return! execute connection "INSERT into transmitted (raw_packet, packet_type)
                                        VALUES (@raw_packet, @packet_type);" frame
        }

    let getTransmit connectionString : Task<Result<RawTransmitted seq, exn>> =
        task {
            use connection = new SqliteConnection(connectionString)
            return! query connection "SELECT rowid, date_created, raw_packet, packet_type, transmitted 
                                      FROM transmitted
                                      WHERE transmitted = 0;" None            
        }

    let setTransmitted connectionString (idList: int list) : Task<Result<int,exn>> list =
        let execute (txId:TransmittedId) =            
            task {
                use connection = new SqliteConnection(connectionString)
                return! execute connection "UPDATE transmitted
                                            SET transmitted = 1
                                            WHERE rowid = (@id);" txId
            }
        idList |> List.map (fun i -> { id = i }) |> List.map execute

    let writeFramesToKissUtil (connectionString:String) (path:string) =
        task {
            let! transmit = getTransmit connectionString
            match transmit with
            | Ok result -> return ((result |> Seq.map (fun r -> r.raw_packet) |> Seq.toList |> writeFrameToKissUtil None path) , (result |> Seq.map (fun r -> r.rowid))) |> Ok                      
            | Error exn -> return Error exn
        }

    let saveRawReceivedFrame connectionString (frame:RawReceivedFrameRecord) : Task<Result<int,exn>> =
        task { //datetime('now')
            use connection = new SqliteConnection(connectionString)
            return! execute connection "INSERT INTO received(raw_packet, packet_type, error)
            VALUES (@raw_packet, @packet_type, @error);" frame
        }

    let informationType frame =
        match (|Information|_|) frame with
        | Some i    -> TNC2MON.getRawPaketType(i.Substring(0, 1)).ToString()
        | None      -> "No information part found."

    let getRecords path (file: string option) =
            let d = new DirectoryInfo(path);//Assuming Test is your Folder
            let files = d.GetFiles()  //GetFiles("*.txt"); //Getting Text files
            let getRecords fileName = File.ReadAllLines (Path.Combine(path, fileName))
            match file with
            | Some f -> getRecords f
            | None   -> files |> Array.map (fun f -> f.Name) |> Array.map (fun f -> getRecords f) |> Array.head
        
    let saveReveivedRawRecords connectionString path (file: string option) =
        let rcrds = 
            match file with
            | None -> getRecords path None
            | Some f -> getRecords path (Some f)

        let mapToRawFrameRecord error (frame:string)  =
            {
                raw_packet = frame
                packet_type = informationType frame
                error = error
            }
        let doSave frame =
            task {
                return! (saveRawReceivedFrame connectionString frame)
            }           
        let saveFrame rcrd = 
            match (|Frame|_|) rcrd with
            | Some f -> f |> (mapToRawFrameRecord String.Empty) |> (fun f -> (doSave f).Result)
            | None   -> mapToRawFrameRecord rcrd "Record not in expected format." |> (fun f -> (doSave f).Result)
 
        rcrds |> Array.map saveFrame
