// Learn more about F# at http://fsharp.org

open System
open Argu
open CommandArguments
open chickadee.core.TNC2MON
open chickadee.core
open chickadee.infrastructure.DireWolf.KissUtil

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

    let composePositionReportMessage (pRpt: ParseResults<PositionReportArguments>) : PositionReport.PositionReport = 
        let latArgu = pRpt.GetResult(CommandArguments.Latitude)
        let lonArgu = pRpt.GetResult(CommandArguments.Longitude)
        let lat     = PositionReport.FormattedLatitude.create latArgu
        let lon     = PositionReport.FormattedLongitude.create lonArgu
        let symbol  = SymbolCode.fromSymbol ((pRpt.TryGetResult(CommandArguments.Symbol)) |> Option.defaultValue '-')
        let comment = pRpt.TryGetResult(CommandArguments.Comment) |> Option.defaultValue String.Empty
        { 
            Position = { Latitude = lat; Longitude = lon }
            Symbol = (if symbol.IsSome then symbol.Value else SymbolCode.House)
            TimeStamp = Some (chickadee.core.Timestamp.TimeStamp.create chickadee.core.Timestamp.TimeZone.Local)
            Comment = PositionReport.PositionReportComment.create comment
        }

    let composeMessage (msg:ParseResults<CustomMessageArguments>) : Message.Message =
        let addr = msg.GetResult(CommandArguments.Addressee)
        let message = msg.GetResult(CommandArguments.Message)
        let addressee = "ADDRESSEE cannot be empty and must be 1 - 9 characters."
        let msgText = "MESSAGE TEXT must be less than 68 characters and cannot contain | ~"
        let msgNum = "MESSAGE NUMBER must be less than 10"
        match CallSign.create addressee, Message.MessageText.create message, Message.MessageNumber.create String.Empty with
        | Some c, Some m, Some n -> {
                                        Addressee = c
                                        MessageText = m
                                        MessageNumber = n
                                    }
        | None, Some _, Some _ -> failwith addressee //TODO use a proper flow/pipeline with result type instead?
        | None, None, Some _ -> failwith (sprintf "%s %s" addr msgText)
        | None, Some _, None -> failwith (sprintf "%s %s" addr msgNum)
        | Some _, None, None -> failwith (sprintf "%s %s" msgText msgNum)
        | Some _, Some _, None -> failwith msgNum
        | Some _, None, Some _ -> failwith msgText
        | None, None, None -> failwith (sprintf "%s %s %s" addr msgText msgNum)

    [<EntryPoint>]
    let main argv =
        let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
        let parser = ArgumentParser.Create<SourcePathArguments>(programName = "chick", errorHandler = errorHandler)

        try
            let results = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)
            printfn "Got parse results %A" <| results.GetAllResults()

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

            let information =
                match pRpt, msg with
                | Some _, Some _        -> failwith "Cannot use both Position Report and Custom Message at the same time."
                | Some rptArgs, None    -> (composePositionReportMessage rptArgs) 
                                           |> PositionReport.PositionReportFormat.PositionReportWithoutTimeStampWithMessaging //todo support all types
                                           |> Information.PositionReport  
                | None _, Some msg      -> (composeMessage msg) 
                                           |> Message.MessageFormat.Message 
                                           |> Information.Message //Unformatted (UnformattedMessage.create msg)
                | None, None            -> failwith "Must provide a position report or a message."
            
            let senderCallSign = 
                match CallSign.create sender with
                | Some c -> c
                | None -> failwith "SENDER cannot be empty and must be 1 - 9 characters. See APRS 1.01."

            let destCall =
                match CallSign.create destination with
                | Some c -> c
                | None -> failwith "DESTINATION cannot be empty and must be 1 - 9 characters. See APRS 1.01."

            let packet =
                {
                    Sender      = senderCallSign
                    Destination = destCall
                    Path        = WIDEnN WIDE11
                    Information = Some information
                }

            let txDelay =                 
                //Some [ TxDelay 0; TxDelay 0; ] //2 seconds in 10 ms units
                None

            let file = 
                match saveTo with
                | Some path -> writePacketsToKissUtil txDelay path [packet]
                | None      -> String.Empty
            Console.WriteLine file
        with e ->
            Console.error <| (sprintf "%s" e.Message)

        0