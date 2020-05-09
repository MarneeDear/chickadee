namespace chickadee.infrastructure

open chickadee.core.PositionReport
open chickadee.core.Message
open chickadee.core.DataFormats.MessageActivePatterns
open chickadee.core.DataFormats.PositionReportActivePatterns
open chickadee.core.TNC2MonActivePatterns
open chickadee.core.Participant
open chickadee.core.DataFormats.DataFormatType
open FSharp.Control.Tasks
open System.Threading.Tasks
open Microsoft.Data.Sqlite
open chickadee.infrastructure.database


//TODO Kiss settings
(*
Input, starting with a lower case letter is interpreted as being a command. 
Whitespace, as shown in the examples, is optional.
letter meaning example
------- ----------- -----------
d txDelay, 10ms units d 30
p Persistence p 63
s Slot time, 10ms units s 10
t txTail, 10ms units t 5
f Full duplex f 0
h set Hardware h (hardware-specific)
*)

//TODO do this async maybe?
module TNC2MONRepository =
    open System.IO
    open chickadee.core
        
    let mapPositionReport rpt (rawType:APRSDataFormats.DataFormat) =
        let posResult =
            let posRec lat lon = 
                {
                    Latitude = FormattedLatitude.check lat //TODO how to create this when the latiude should have already been formatted? How to check?
                    Longitude = FormattedLongitude.check lon 
                }
            match (|Latitude|_|) rpt, (|Longitude|_|) rpt with
            | Some lat, Some lon    -> Ok (posRec lat lon)
            | None, Some _          -> Error "Latitude was not in expected format."
            | Some _, None          -> Error "Longitude was not in exptected format."
            | None, None            -> Error "Neither Latitude nor Longitude were in expected format."
        let symbolCode =
            match (|Symbol|_|) rpt with
            | Some s    -> s //Defaults to house if no match found -- TODO do I want to do this?
            | None      -> Common.SymbolCode.House

        let comment = //TODO handle the case where the comment is not accepted length
            let sym, dstcall = symbolCode.ToCode()

            match (|Comment|_|) sym rpt with
            | Some c    -> PositionReportComment.create c
            | None      -> None //PositionReportComment.create String.Empty

        let positionReport (p:PositionReport) =
            match rawType with
            | APRSDataFormats.DataFormat.PositionReportWithoutTimeStampOrUltimeter    -> PositionReportWithoutTimeStampOrUltimeter p |> Ok
            | APRSDataFormats.DataFormat.PositionReportWithTimestampNoMessaging       -> PositionReportWithTimestampNoMessaging p |> Ok
            | APRSDataFormats.DataFormat.PositionReportWithoutTimeStampWithMessaging  -> PositionReportWithTimestampWithMessaging p |> Ok
            | APRSDataFormats.DataFormat.PositionReportWithTimestampWithMessaging     -> PositionReportWithTimestampWithMessaging p |> Ok
            | _ -> Error "Type must be a position report."

        match posResult with
        | Ok p ->   {
                        Position    = p
                        Symbol      = symbolCode
                        TimeStamp   = None
                        Comment     = comment 
                        //TODO is this right
                    } |> positionReport |> Result.bind (fun p -> Ok (TNC2MON.Information.PositionReport p))  //PositionReport.PositionReportWithoutTimeStampWithMessaging
        | Error msg -> Error msg

    let mapMessage (msg:string) = //Can I do this recursivley
        match (|Addressee|_|) msg, (|Message|_|) msg, (|MessageNumber|_|) msg with
        | Some a, Some m, Some n -> {
                                        Addressee = a
                                        MessageText = m
                                        MessageNumber = n
                                     } |> MessageFormat.Message |> TNC2MON.Information.Message |> Ok
        | None, Some _, Some _ -> Error "Addressee part of message not in expected format."
        | Some _, None, Some _ -> Error "Message part of message not in expected format."
        | Some _, Some _, None -> Error "Message Number part of message not in expected format."
        | _, _, _              -> Error "Message not in expected format."

    let mapUnsupportedMessage (msg:string) =
        TNC2MON.Information.Unsupported msg //(UnformattedMessage.create msg)

    //18 USER-DEFINED DATA FORMAT --experimental designator
    //APRS 1.01 For experimentation, or prior to being issued a User ID, anyone may utilize
    //the User ID character of { without prior notification or approval (i.e. packets
    //beginning with {{ are experimental, and may be sent by anyone).
    let mapParticipantReport (rpt:string) =
        match rpt.Substring(0, 2) with
        | id when id.Equals("{P")    ->
                                        if rpt.Length >= 15 && rpt.Length <= 253 then
                                            let timestamp = RecordedOn.create (Some (RecordedOn.revert (rpt.Substring(2, 8))))
                                            let id = ParticipantID.create (rpt.Substring(10, 5))
                                            let st1 = int (rpt.Substring(15, 1))
                                            let st2 = int (rpt.Substring(16, 1))
                                            let msg = rpt.Substring(17)

                                            //let cancelled = match rpt.Substring(rpt.Length - 2, 1) with
                                            //                | "C"   -> true 
                                            //                | _     -> false

                                            let psts =
                                                 ParticipantStatus.fromStatusCombo (st1, st2, msg)
                                            {
                                                TimeStamp = timestamp
                                                ParticipantID = id.Value
                                                ParticipantStatus = psts
                                                //Cancelled = false                
                                            }                                            
                                            |> TNC2MON.Information.ParticipantStatusReport |> Ok
                                        else 
                                            Error "Participant report not in expected format. Message length exceeded 253 characters."
        | _                         -> Error "Participant report not in expected format. Message did not start with expected identifier -- {P."

    //Examples
    //[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
    //[0] KG7SIO-7>APRD15,WIDE1-1:=3216.4N/11057.3Wb
    //TODO use ROP and a pipeline -- how best to do that?
    (*
        Ident Data Type 
        0x1c Current Mic-E Data (Rev 0 beta) 
        < Station Capabilities
        0x1d Old Mic-E Data (Rev 0 beta) 
        = Position without timestamp (with APRS messaging)
        ! Position without timestamp (no APRS messaging), or Ultimeter 2000 WX Station
        > Status
        double-quote [Unused] 
        ? Query
        # Peet Bros U-II Weather Station 
        @ Position with timestamp (with APRS messaging)
        $ Raw GPS data or Ultimeter 2000 A–S [Do not use]
        % Agrelo DFJr / MicroFinder T Telemetry data
        & [Reserved — Map Feature] U–Z [Do not use]
        ' Old Mic-E Data (but Current data for TM-D700) 
        [ Maidenhead grid locator beacon (obsolete)
        ( [Unused]  //
        \ [Unused]
        ) Item 
        ] [Unused]
        * Peet Bros U-II Weather Station 
        ^ [Unused]
        + [Reserved — Shelter data with time] 
        _ Weather Report (without position)
        , Invalid data or test data 
        ‘ Current Mic-E Data (not used in TM-D700)
        - [Unused] a–z [Do not use]
        . [Reserved — Space weather] 
        { User-Defined APRS packet format
        / Position with timestamp (no APRS messaging) 
        | [Do not use — TNC stream switch character]
        0–9 [Do not use] 
        } Third-party traffic
        : Message ~ [Do not use — TNC stream switch character]
        ; Object
    *)
    let convertFrameToAPRSData (frame:string) =
        let information frame =
            match (|Information|_|) frame with
            | Some m    -> m |> Ok
            | None      -> "No information part found." |> Error

        let data (info:string) =
            //match TNC2MON.getRawPaketType(info.Substring(0, 1)) with
            match (|FormatType|_|) info with
            | Some APRSDataFormats.DataFormat.Message -> mapMessage (info.Substring 1)
            | Some APRSDataFormats.DataFormat.PositionReportWithoutTimeStampWithMessaging -> mapPositionReport info APRSDataFormats.DataFormat.PositionReportWithoutTimeStampWithMessaging //We have a lat/lon position report without timestamot. Let's try to parse it.
            | Some APRSDataFormats.DataFormat.UserDefined -> //Ok (mapParticipantReport (msg.Substring(1))) //We have user-defined data. Maybe it's a participant report. Let's try to parse it
                                               mapParticipantReport (info.Substring 1)
            | Some APRSDataFormats.DataFormat.Unsupported -> mapUnsupportedMessage(info.Substring 1) |> Ok //if not in supported format just turn it into a message so it can be logged
            | _ -> mapUnsupportedMessage(info.Substring 1) |> Ok

        frame
        |> information
        |> Result.bind data

    let convertRecordToAPRSData (record:string) =
        let frame rcrd =
            match (|Frame|_|) rcrd with
            | Some f    -> f |> Ok
            | None      -> "Frame not in expected format." |> Error
        
        (frame record)
        |> Result.bind convertFrameToAPRSData 
        
    let convertToPacket (record:string) =
        let frame rcrd =
            match (|Frame|_|) rcrd with
            | Some f    -> f |> Ok
            | None      -> "Frame not in expected format." |> Error
        
        let address frame =
            match (|Address|_|) frame with
            | Some a -> a |> Ok
            | None -> "No address part found." |> Error

        let destination addrs =
            match (|Destination|_|) addrs with
            | Some d -> d |> Ok
            | None -> "No destination part found." |> Error

        let sender addrs =
            match (|Sender|_|) addrs with
            | Some s -> s |> Ok
            | None -> "No sender part found." |> Error

        let path addrs =
            match (|Path|_|) addrs with
            | Some p -> p |> Ok
            | None -> "No path found." |> Error

        match frame record with
        | Ok f -> 
                  match (convertRecordToAPRSData record), (address f) with
                  | Ok i, Ok a -> 
                                  match (destination a), (sender a), (path a) with
                                  | Ok d, Ok s, Ok p -> { 
                                                          TNC2MON.Packet.Sender = if (CallSign.create s).IsSome then (CallSign.create s).Value else (CallSign.create System.String.Empty).Value
                                                          TNC2MON.Packet.Destination = if (CallSign.create d).IsSome then (CallSign.create d).Value else (CallSign.create System.String.Empty).Value
                                                          TNC2MON.Packet.Path = WIDEnN WIDE11 //TODO path is a list
                                                          TNC2MON.Packet.Information = Some i
                                                        } |> Ok
                                  | Error m1, Error m2, Error m3 -> sprintf "Could not parse record [%s]. ERRORS [%s] [%s] [%s]" record m1 m2 m3 |> Error
                                  | Error m1, Error m2, Ok p -> sprintf "Could not parse record [%s]. ERRORS [%s] [%s]" record m1 m2 |> Error
                                  | Error m1, Ok s, Error m3 -> sprintf "Could not parse record [%s]. ERRORS [%s] [%s]" record m1 m3 |> Error
                                  | Error m1, Ok s, Ok p -> sprintf "Could not parse record [%s]. ERROR [%s]" record m1 |> Error
                                  | Ok d, Error m2, Ok p -> sprintf "Could not parse record [%s]. ERROR [%s]" record m2 |> Error
                                  | Ok d, Error m2, Error m3 -> sprintf "Could not parse record [%s]. ERROR [%s] [%s]" record m2 m3 |> Error                                  
                                  | Ok d, Ok s, Error m3 -> sprintf "Could not parse record [%s]. ERROR [%s]" record m3 |> Error
                  | Error m1, Ok a -> sprintf "Could not parse record [%s]. ERROR [%s]" record m1 |> Error
                  | Ok i, Error m2 -> sprintf "Could not parse record [%s]. ERROR [%s]" record m2 |> Error
                  | Error m1, Error m2 -> sprintf "Could not parse record [%s]. ERROR [%s] [%s]" record m1 m2 |> Error
        | Error m -> sprintf "Could not parse record [%s]. ERROR [%s]" record m |> Error

    (*   

        /092345z4903.50N/07201.75W>Test1234

        POSTION example
        4903.50N/07201.75W

        LATTITUDE
        4903.50N is 49 degrees 3 minutes 30 seconds north.
        01234567
        49 degress
        3 minutes
        30 seconds

        LONGITUDE
        07201.75W is 72 degrees 1 minute 45 seconds west.
        012345678
        72 degrees
        1 minues
        45 seconds


    *)

    let convertToLat (formattedLat:FormattedLatitude) =
        let lat = FormattedLatitude.value formattedLat
        let deg = float (lat.Substring(0,2).TrimStart([|'0'|]))
        let min = float (lat.Substring(2, 2).TrimStart([|'0'|]))
        let sec = float (lat.Substring(4,3))
        let hem = lat.Substring(7,1)

        let sign =
            match hem with
            | "N" -> 1.0
            | "S" -> -1.0
            | _ -> 0.0
        

        (deg + (min/60.0) + (sec * 60.0)/3600.0) * sign

    let convertToLon (formattedLon:FormattedLongitude) =
        let lon = FormattedLongitude.value formattedLon
        let deg = float (lon.Substring(0,3).TrimStart([|'0'|]))
        let min = float (lon.Substring(3, 2).TrimStart([|'0'|]))
        let sec = float (lon.Substring(5,3))
        let hem = lon.Substring(8,1)
        let sign =
            match hem with
            | "W" -> -1.0
            | "E" -> 1.0
            | _ -> 0.0        
        
        (deg + (min/60.0) + (sec * 60.0)/3600.0) * sign

    let convertPoitionToCoordinates (posRpt:PositionReport.PositionReport) =
        (convertToLat posRpt.Position.Latitude) , (convertToLon posRpt.Position.Longitude)


    
    ////All received frames are displayed in the usual monitor format, preceded with the channel number inside of [ ].
    ////[0] K1NRO-1>APDW14,WIDE2-2:!4238.80NS07105.63W#PHG5630
    ////See Dire Wolf User Guide 14.6 kissutil – KISS TNC troubleshooting and Application Interface
    ////TODO this needs to process each file not a specific file name -- we wont know file name at runtime
    //let processKissUtilFrames path (file: string option) =
    //    let d = new DirectoryInfo(path);//Assuming Test is your Folder
    //    let files = d.GetFiles()  //GetFiles("*.txt"); //Getting Text files
    //    let getFrames fileName = 
    //        File.ReadAllLines (Path.Combine(path, fileName))
    //        |> Array.map (fun f -> convertRecordToAPRSData f) //TODO Should convert to a packets
    //    match file with
    //    | Some f    -> getFrames f
    //    | None      -> files 
    //                    |> Array.map (fun f -> f.Name)
    //                    |> Array.map getFrames
    //                    |> Array.head

    //let getRecords path (file: string option) =
    //    let d = new DirectoryInfo(path);//Assuming Test is your Folder
    //    let files = d.GetFiles()  //GetFiles("*.txt"); //Getting Text files
    //    let getRecords fileName = 
    //        File.ReadAllLines (Path.Combine(path, fileName))
    //    match file with
    //    | Some f -> getRecords f
    //    | None   -> files |> Array.map (fun f -> f.Name) |> Array.map (fun f -> getRecords f) |> Array.head
        
    //let saveReveivedRawMessages connectionString path =
    //    let rcrds = getRecords path None
    //    let informationType frame =
    //        match (|Information|_|) frame with
    //        | Some i    -> TNC2MON.getRawPaketType(i.Substring(0, 1)).ToString()
    //        | None      -> "No information part found."

    //    let mapToRawFrameRecord error (frame:string)  =
    //        {
    //            raw_packet = frame
    //            packet_type = informationType frame
    //            error = error
    //        }
    //    let saveFrame rcrd = 
    //        match (|Frame|_|) rcrd with
    //        | Some f -> f |> (mapToRawFrameRecord String.Empty) |> (saveRawFrame connectionString)
    //        | None   -> mapToRawFrameRecord rcrd "Record not in expected format." |> (saveRawFrame connectionString)
 
    //    rcrds |> Array.map saveFrame
    //    //|> Array.iter save

    //    //let getRawType packet = 
    //    //let packetsWithType (packet:Result<TNC2MON.Packet, string>) =
    //    //    match packet with
    //    //    | Ok p -> (p, p.Information.GetType().ToString())
    //    //    | Error m -> "No type found."
    //    //let results = packets |> Array.map(fun p -> p, (packetsWithType p))
