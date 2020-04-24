namespace All

open Saturn
open Giraffe
open Config
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Models
open chickadee.infrastructure.DireWolf

module Controller =

    let indexAction (ctx: HttpContext) = 
        let cnf = Controller.getConfig ctx
        let logger = ctx.GetLogger()

        task {
            let! txResult = KissUtil.getTransmitFrames cnf.connectionString None None
            let! rxResult = KissUtil.getReceivedFrames cnf.connectionString None
            let mapToTx () =
                match txResult with
                | Ok txs -> txs |> Seq.map (fun t -> {
                                                    DateCreated = t.date_created
                                                    RawPacket = t.raw_packet
                                                    PacketType = t.packet_type
                                                    Transmitted = if t.transmitted = 0 then false else true
                                                })
                                |> Seq.toList
                                |> Ok
                | Error msg -> Error msg
            
            let mapToRx () =
                match rxResult with
                | Ok rxs -> rxs |> Seq.map (fun r -> {
                                                        DateCreated = r.date_created
                                                        RawPacket = r.raw_packet
                                                        PacketType = r.packet_type
                                                        Error = r.error
                                                    })
                                |> Seq.toList
                                |> Ok
                | Error msg -> Error msg

            match mapToRx (), mapToTx () with
            | Ok rx, Ok tx -> return Views.index (Some rx) (Some tx) None None
            | Error rErr, Ok tx ->  logger.LogError(rErr.Message)
                                    return Views.index (None) (Some tx) (Some rErr.Message) None
            | Ok rx, Error tErr ->  logger.LogError(tErr.Message)
                                    return Views.index (Some rx) (None) (None) (Some tErr.Message)
            | Error rErr, Error tErr -> logger.LogError(sprintf "[%s] [%s]" rErr.Message tErr.Message)
                                        return Views.index (None) (None) (Some rErr.Message) (Some tErr.Message)

        }

    let resource = controller {
        index indexAction
    }
