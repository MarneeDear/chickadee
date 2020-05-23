namespace Messages

open Saturn
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Model
open chickadee.infrastructure.DireWolf
open chickadee.core.TNC2MON
open chickadee.core
open chickadee.core.Message
open chickadee.core.DataFormats
open Config

module Controller =

    let indexAction (ctx: HttpContext) = 
        let cnf = Controller.getConfig ctx
        let logger = ctx.GetLogger()

        task {
            //TODO get list of received and sent messages
            let! txResult = KissUtil.getTransmitFrames cnf.connectionString None (Some "Message")
            let! rxResult = KissUtil.getReceivedFrames cnf.connectionString (Some "Message")
            
            let getInformation (f:string) =
                match f with
                | TNC2MonActivePatterns.Information info -> info.Substring(1)
                | _ -> "No information part found"

            let getAddressee (f:string) =
                match getInformation f with
                | MessageActivePatterns.Addressee a -> CallSign.value a
                | _ -> "No addressee found"

            let getMessage (f:string) =
                match getInformation f with
                | MessageActivePatterns.Message m -> MessageText.value m
                | _ -> "No message text found"

            let getMessageNum (f:string) =
                match getInformation f with
                | MessageActivePatterns.MessageNumber mn -> MessageNumber.value mn
                | _ -> "No message number found"

            let rxs =      
                match rxResult with
                | Ok rxs -> rxs |> Seq.map (fun r -> {
                                                        DateCreated = r.date_created
                                                        RawPacket = r.raw_packet
                                                        Addressee = getAddressee r.raw_packet
                                                        MessageText = getMessage r.raw_packet
                                                        MessageNumber = getMessageNum r.raw_packet
                                                        Error = System.String.Empty
                                                     }) 
                | Error exn ->  logger.LogError(exn.Message)
                                seq [] //TODO show errors on the view

            let txs = 
                match txResult with
                | Ok txs -> txs |> Seq.map (fun r -> {
                                                        DateCreated = r.date_created
                                                        RawPacket = r.raw_packet
                                                        Addressee = getAddressee r.raw_packet
                                                        MessageText = getMessage r.raw_packet
                                                        MessageNumber = getMessageNum r.raw_packet
                                                        Error = System.String.Empty
                                                        Transmitted = if r.transmitted = 0 then false else true
                                                     }) 
                | Error exn ->  logger.LogError(exn.Message)
                                seq [] //TODO show errors on the view
            return Views.index rxs txs
        }

    let addAction (ctx:HttpContext) =
        let cnf = Controller.getConfig ctx
        let logger = ctx.GetLogger()

        task {
            return Views.add ctx
        }

    let createAction (ctx:HttpContext) =
        let cnf = Controller.getConfig ctx
        let logger = ctx.GetLogger()
        task {
            let! model = Controller.getModel<Model.Message> ctx
            //TODO add to database and allow background job to handle writing to DireWolf
            //Build the packets
            //TODO handle when any part of the message fails
            let msg : Message = {
                Addressee = (CallSign.create model.Addressee).Value
                MessageText = (MessageText.create model.MessageText).Value
                MessageNumber = (MessageNumber.create model.MessageNumber)
            }

            let packet : Packet = {
                Sender = (CallSign.create "KG7SIO").Value //TODO this should be the person using the system
                Destination = (CallSign.create "APDW15").Value
                Path = Path.WIDEnN WIDE11 //TODO support more paths
                Information = Some (msg |> MessageFormat.Message |> Information.Message)
            }
            KissUtil.savePacketToDatabase cnf.connectionString packet |> ignore //TODO handle errors
            return! Controller.redirect ctx "/messages"
        }

    let resource = controller {
        index indexAction
        add addAction
        create createAction
    }
