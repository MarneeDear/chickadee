module DireWolfTests

open Expecto
open chickadee.core.TNC2MON
open chickadee.core.Common
open chickadee.infrastructure.DireWolf.KissUtil
open System
open System.IO
open chickadee.core.PositionReport
open FSharp.Control.Tasks

[<Literal>]
let CONN = "DataSource=database.sqlite"

[<Literal>]
//let FILE_PATH = @"."
let FILE_PATH = @"."
[<Literal>]
let REC = "REC"
[<Literal>]
let XMIT = "XMIT"

[<Literal>]
let SENDER = "kg7sio"
[<Literal>]
let DESTINATION = "apdw15"  //DireWolf v1.15 ToCall 

let txFrame : RawTransmittedFrame =
    {
        date_created = ""
        rowid = 0
        raw_packet = "KG7SIO>APDW15,WIDE1-1:=3603.33N/11206.34W-"
        packet_type = "Position Report without timestamp or Ultimeter station"
        transmitted = 0
    }

let saveTx () =
    (saveTransmitFrame CONN txFrame).Result |> ignore
    (saveTransmitFrame CONN txFrame).Result |> ignore
    (saveTransmitFrame CONN txFrame).Result |> ignore
    (saveTransmitFrame CONN txFrame).Result |> ignore
    (saveTransmitFrame CONN txFrame).Result |> ignore
    (saveTransmitFrame CONN txFrame).Result |> ignore

[<Tests>]
let WriteFramesTests =
    testList "Save a raw frame to the database" [
        testCase "Recevied frames are saved to the database" <| fun _ ->
            let rawFrame : RawReceivedFrame = 
                {
                    date_created = ""
                    raw_packet = "KG7SIO>APDW15,WIDE1-1:=3603.33N/11206.34W-"
                    packet_type = "Position Report without timestamp or Ultimeter station"
                    error = ""
                }
            let result = (saveRawReceivedFrame CONN rawFrame).Result
            Expect.isOk result "Recevied frame was not saved"
        testCase "Can save frame to be transmitted" <| fun _ ->
            let result = (saveTransmitFrame CONN txFrame).Result
            Expect.isOk result "Tx frame was not saved."
        testCase "Can set frames to transmitted2" <| fun _ ->
            saveTx ()
            let result = (getTransmitFrames CONN (Some true) None).Result
            Expect.isOk result "Error getting transmit records"
            match result with
            | Ok txs -> let ids = txs |> Seq.map (fun t -> t.rowid) |> Seq.toList
                        let tResult = ids |> (setTransmitted CONN)
                        Expect.isOk tResult.Result "Transmitted records were not updated"
            | Error _ -> ()
            //TODO check transmit was set?
        testCase "Can write frames from database to kissutil" <| fun _ ->
            saveTx ()
            let result = (writeStoredFramesToKissUtil CONN None XMIT)
            Expect.isOk result "Kiss util transmit file was not created"
            //TODO check file was created correctly?
    ]
