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
open Console
open PrintOutput

(*

EXAMPLES: 
$ dotnet run --sender KG7SIO --destination KG7SIL --positionreport latitude 3216.4 longitude 11057.3
$ dotnet run --sender KG7SIO --destination KG7SIL --positionreport latitude 3216.4 longitude 11057.3 symbol b comment "Hello world!"
$ dotnet run --sender KG7SIO --destination KG7SIL --rpt latitude 3216.4 N longitude 11057.3 W
 dotnet run --project src/chickadee.cli/ -- --save-to XMIT --sender KG7SIO-7 --parseframe "KG7SIO-7>APDW15,WIDE1-1:/092345z4903.50N/07201.75W>Test1234"
Use the short argument flags and save a kissutl frame to the current directors (.). Use the defualts for symbol (House) and comment
$ dotnet run -s KG7SIO -d KG7SIL --save-to . --rpt latitude 3216.4 N longitude 11057.3 W

On the Pi 3
$ ./ckdee -s KG7SIO -d KG7SIL --save-to ~/faprs-stuff/kiss-output/ --rpt latitude 3216.4 N longitude 11057.3 W

NOTE you can't use - (dash) in symbol because Argu won't parse it

*)

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

            let txDelay =                 
                //Some [ TxDelay 0; TxDelay 0; ] //2 seconds in 10 ms units
                None

            let writePacketToFile (packet:Result<Packet, string list>) = 
                match packet with
                | Ok p -> match saveTo with
                          | Some path -> writePacketsToKissUtil txDelay path [p] |> ignore
                                         Console.info (sprintf "Wrote packet to file %s" path)
                          | None      -> String.Empty |> ignore
                                         Console.error "No save to path provided. Cannot write packet"
                | Error m -> m |> List.iter Console.error
                          
            match pRpt, msg, pFrame with
            | Some rptArgs, None, None  -> let packet = composePositionReportPacket rptArgs senderCallSign destCallSign
                                           packet |> printPacket
                                           packet |> writePacketToFile
            | None, Some msg, None      -> let packet = composeMessagePacket msg senderCallSign destCallSign
                                           packet |> printPacket
                                           packet |> writePacketToFile
            | None, None, Some frame    -> parseFrame frame
                                           |> printPacket
            | None, None, None          -> failwith "Must provide a position report, a message, or frame to parse."
            | _, _, _ -> failwith "Cannot use Position Report, Custom Message, and Parse Frame at the same time. Use only one option at a time."
        with e ->
            Console.error <| (sprintf "%s" e.Message)
            Console.error <| (sprintf "%A" e.InnerException)
            Console.error <| (sprintf "%s" e.StackTrace)

        0