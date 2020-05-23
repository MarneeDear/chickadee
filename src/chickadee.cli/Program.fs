// Learn more about F# at http://fsharp.org

open System
open Argu
open CommandArguments
open chickadee.core.TNC2MON
open chickadee.core
open chickadee.infrastructure.TNC2MONRepository
open chickadee.infrastructure.DireWolf.KissUtil
open PositionReportComposer
open MessageComposer
open FrameParser

(*

EXAMPLES: 
$ dotnet run --sender KG7SIO --destination KG7SIL --positionreport latitude 3216.4 longitude 11057.3
$ dotnet run --sender KG7SIO --destination KG7SIL --positionreport latitude 3216.4 longitude 11057.3 symbol b comment "Hello world!"
$ dotnet run --sender KG7SIO --destination KG7SIL --rpt latitude 3216.4 N longitude 11057.3 W

Use the short argument flags and save a kissutl frame to the current directors (.). Use the defualts for symbol (House) and comment
$ dotnet run -s KG7SIO -d KG7SIL --save-to . --rpt latitude 3216.4 N longitude 11057.3 W

On the Pi 3
$ ./faprs.cli -s KG7SIO -d KG7SIL --save-to ~/faprs-stuff/kiss-output/ --rpt latitude 3216.4 N longitude 11057.3 W

NOTE you can't use - (dash) in symbol because Argu won't parse it

*)

module Console =

    let log =
        let lockObj = obj()
        fun color s ->
            lock lockObj (fun _ ->
                Console.ForegroundColor <- color
                printfn "%s" s
                Console.ResetColor())

    let complete    = log ConsoleColor.Magenta
    let ok          = log ConsoleColor.Green
    let info        = log ConsoleColor.Blue
    let warn        = log ConsoleColor.Yellow
    let error       = log ConsoleColor.Red

module Main =

    [<EntryPoint>]
    let main argv =
        let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
        let parser = ArgumentParser.Create<SourcePathArguments>(programName = "chick", errorHandler = errorHandler)

        try
            let results = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)
            Console.info (sprintf "This is what you want me to do %A" <| results.GetAllResults())

            let saveTo = results.TryGetResult(CommandArguments.SaveFilePath)
            
            let sender = results.GetResult(Sender)
            let destination = 
                let r = results.TryGetResult(Destination)
                match r with
                | Some d    -> d
                | None      -> "APDW15"

            let path = Common.Path.WIDEnN //only this for now TODO

            let pRpt = results.TryGetResult(CommandArguments.PositionReport)
            let msg = results.TryGetResult(CommandArguments.CustomMessage)
            let pFrame = results.TryGetResult(CommandArguments.ParseFrame)

            let senderCallSign = 
                match CallSign.create sender with
                | Some c -> c
                | None -> failwith "SENDER cannot be empty and must be 1 - 9 characters. See APRS 1.01."

            let destCallSign =
                match CallSign.create destination with
                | Some c -> c
                | None -> failwith "DESTINATION cannot be empty and must be 1 - 9 characters. See APRS 1.01."

            let packet =
                match pRpt, msg, pFrame with
                | Some _, Some _, Some _    -> failwith "Cannot use Position Report, Custom Message, and Parse Frame at the same time. Use only one option at a time."
                | Some _, Some _, None      -> failwith "Cannot use Position Report, Custom Message, and Parse Frame at the same time. Use only one option at a time."
                | Some rptArgs, None, None  -> composePositionReportPacket rptArgs senderCallSign destCallSign
                | None, Some msg, None      -> composeMessagePacket msg senderCallSign destCallSign
                | None, None, Some frame    -> parseFrame frame
                | None, None, None          -> failwith "Must provide a position report, a message, or frame to parse."

            //let packet =
            //    {
            //        Sender      = senderCallSign
            //        Destination = destCall
            //        Path        = WIDEnN WIDE11
            //        Information = Some information
            //    }

            let txDelay =                 
                //Some [ TxDelay 0; TxDelay 0; ] //2 seconds in 10 ms units
                None

            //let file = 
            //    match saveTo with
            //    | Some path -> writePacketsToKissUtil txDelay path [packet]
            //    | None      -> String.Empty
            //Console.WriteLine file
            //Console.ok (packet.ToString())

            let printMessage (msg: Message.MessageFormat) =
                () //TODO

            let printPositionReport (posRpt:PositionReport.PositionReportFormat) =
                let printConvertedLatLong (pos:PositionReport.PositionReport) =
                    let latD, lonD = convertPoitionToCoordinates pos
                    Console.ok (sprintf "LATITUDE and LONGITUDE in DECIMAL FORMAT: (%f, %f)" latD lonD)
                let printPos (pos:PositionReport.PositionReport) =
                    //Console.ok (sprintf "APRS POSITION: %s" (pos.Position.ToString()))
                    printConvertedLatLong pos
                    Console.ok (sprintf "SYMBOL: %A" (pos.Symbol))
                    match pos.TimeStamp with
                    | Some t -> Console.ok (sprintf "TIME STAMP %s" (Timestamp.TimeStamp.value t))
                    | None -> Console.ok "NO TIMESTAMP"
                    match pos.Comment with
                    | Some c -> Console.ok (sprintf "MESSAGE: %s" (PositionReport.PositionReportComment.value c))
                    | None -> Console.ok "NO MESSAGING"
                match posRpt with
                | PositionReport.PositionReportFormat.PositionReportWithoutTimeStampOrUltimeter p -> printPos p
                | PositionReport.PositionReportFormat.PositionReportWithoutTimeStampWithMessaging p -> printPos p
                | PositionReport.PositionReportFormat.PositionReportWithTimestampNoMessaging p -> printPos p
                | PositionReport.PositionReportFormat.PositionReportWithTimestampWithMessaging p -> printPos p

            //:KB2ICI-14:ack003
            let printMessage (msg:Message.MessageFormat) =
                match msg with
                | Message.MessageFormat.Message m -> Console.ok (sprintf "ADDRESSEE: %A" m.Addressee)
                                                     Console.ok (sprintf "MESSAGE: %A" m.MessageText)
                                                     Console.ok (sprintf "NUMBER: %A" m.MessageNumber)
                | Message.MessageFormat.MessageAcknowledgement ma -> Console.ok "This is a message acknowledgement."
                | Message.MessageFormat.MessageRejection mr -> Console.ok "This is a message rejection."
                | Message.MessageFormat.Announcement a -> Console.ok "This is an announcement."
                | Message.MessageFormat.Bulletin b -> Console.ok "This is a bulletin."
                //| _ -> Console.info "TODO display all Message formats. Your Message format has not yet been coded."

            match packet with
            | Ok p -> Console.complete "Successfully parsed your packet. Here is what I got."
                      Console.ok (sprintf "APRS PACKET: %s" (p.ToString()))
                      Console.ok (sprintf "SENDER : %A" p.Sender)
                      Console.ok (sprintf "DESTINATION : %A" p.Destination)
                      
                      match p.Information with
                      | Some info -> Console.ok (sprintf "INFORMATION : %A" (p.Information.Value.ToString()))
                                     match info with
                                     | Information.Message msg -> printMessage msg
                                     | Information.PositionReport prpt ->  printPositionReport prpt
                                     | _ -> Console.info "This is not an APRS format that I support at the moment."
                      | None -> Console.error "No INFORMATION part found."
                                                    
            | Error errorList -> errorList |> List.iter (fun e -> Console.error e)            
        with e ->
            Console.error <| (sprintf "%s" e.Message)
            Console.error <| (sprintf "%A" e.InnerException)
            Console.error <| (sprintf "%s" e.StackTrace)

        0