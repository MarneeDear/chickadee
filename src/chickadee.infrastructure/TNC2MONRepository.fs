namespace chickadee.infrastructure

open chickadee.core.PositionReport
open chickadee.core.Message
open chickadee.core.DataFormats.MessageActivePatterns
open chickadee.core.DataFormats.PositionReportActivePatterns
open chickadee.core.TNC2MonActivePatterns
open chickadee.core.Participant

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
        
    let mapPositionReport rpt =
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
        let sym =
            match (|Symbol|_|) rpt with
            | Some s    -> s //Defaults to house if no match found -- TODO do I want to do this?
            | None      -> Common.SymbolCode.House
        let comment = //TODO handle the case where the comment is not accepted length
            //let c =
            match (|Comment|_|) (sym.ToChar()) rpt with
            | Some c    -> PositionReportComment.create c
            | None      -> None //PositionReportComment.create String.Empty

        match posResult with
        | Ok p ->   {
                        Position = p
                        Symbol = sym
                        Comment = comment
                    } |> PositionReportType.PositionReportWithoutTimeStamp |> TNC2MON.Information.PositionReport |> Ok
        | Error msg -> Error msg

    let mapMessage (msg:string) = //Can I do this recursivley
        //let partMsg (part, partName) =
        //    match part with
        //    | Some p -> String.Empty
        //    | None -> sprintf "%s%s" partName "part of message not in expected format."
            //match (a, m, n) with
            //| None, Some _, Some _ -> "Addressee"
            //| Some _, None, Some _ -> "Message"
            //| Some _, Some _, None -> "Message Number"

        match (|Addressee|_|) msg, (|Message|_|) msg, (|MessageNumber|_|) msg with
        | Some a, Some m, Some n -> 
                                match CallSign.create a with
                                | Some c -> {
                                                Addressee = c
                                                MessageText = MessageText.create m
                                                MessageNumber = MessageNumber.create n
                                            } |> MessageType.Message |> TNC2MON.Information.Message |> Ok
                                | None -> Error "Addressee call sign not in expected format."
        //| _, _, _ -> [(a, "Addressee"; (m, "Message"); (n, "Message Number")] > List.fold (fun acc elem -> partMsg elem acc)
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
    let convertRecordToAPRSData (record:string) =
        let frame rcrd =
            match (|Frame|_|) rcrd with
            | Some f    -> f |> Ok
            | None      -> "Frame not in expected format." |> Error
        
        let information frame =
            match (|Information|_|) frame with
            | Some m    -> m |> Ok
            | None      -> "No information part found." |> Error
            
            //| id when id.Equals("=") -> mapPositionReport info //(info.Substring 1) //We have a lat/lon position report without timestamot. Let's try to parse it.
            //| id when id.Equals(":") -> mapMessage (info.Substring 1) //|> Ok //we have Message data type. Lets try to parse it
            //| id when id.Equals("{") -> //Ok (mapParticipantReport (msg.Substring(1))) //We have user-defined data. Maybe it's a participant report. Let's try to parse it
            //                            mapParticipantReport (info.Substring 1)
            //                            //let pRpt = (mapParticipantReport (msg.Substring 1))
            //                            //match pRpt with
            //                            //| Some r -> Ok r
            //                            //| None -> Error "Participant report not in expected format"
            //| _                      -> mapUnsupportedMessage(info.Substring 1) |> Ok //if not in supported format just turn it into a message so it can be logged

        let data (info:string) =
            match TNC2MON.getRawPaketType(info.Substring(0, 1)) with
            | TNC2MON.RawInformation.Message -> mapMessage (info.Substring 1)
            | TNC2MON.RawInformation.PositionReportWithoutTimeStampWithMessaging -> mapPositionReport info //We have a lat/lon position report without timestamot. Let's try to parse it.
            | TNC2MON.RawInformation.UserDefined -> //Ok (mapParticipantReport (msg.Substring(1))) //We have user-defined data. Maybe it's a participant report. Let's try to parse it
                                               mapParticipantReport (info.Substring 1)
            | TNC2MON.RawInformation.Unsupported -> mapUnsupportedMessage(info.Substring 1) |> Ok //if not in supported format just turn it into a message so it can be logged

        frame record
        |> Result.bind information
        |> Result.bind data

    let convertToPacket (record:string) =
        let frame rcrd =
            match (|Frame|_|) rcrd with
            | Some f    -> f |> Ok
            | None      -> "Frame not in expected format." |> Error
        
        let information frame =
            match (|Information|_|) frame with
            | Some i    -> i |> Ok
            | None      -> "No information part found." |> Error

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
