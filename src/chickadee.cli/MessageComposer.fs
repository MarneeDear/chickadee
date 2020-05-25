module MessageComposer

open System
open chickadee.core.TNC2MON
open chickadee.core
open Argu
open CommandArguments
open chickadee.core.Message

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
                                    MessageNumber = Some n
                                }
    | None, Some _, Some _ -> failwith addressee //TODO use a proper flow/pipeline with result type instead?
    | None, None, Some _ -> failwith (sprintf "%s %s" addr msgText)
    | None, Some _, None -> failwith (sprintf "%s %s" addr msgNum)
    | Some _, None, None -> failwith (sprintf "%s %s" msgText msgNum)
    | Some _, Some _, None -> failwith msgNum
    | Some _, None, Some _ -> failwith msgText
    | None, None, None -> failwith (sprintf "%s %s %s" addr msgText msgNum)

let composeMessagePacket (msg:ParseResults<CustomMessageArguments>) (sender:CallSign) (destination:CallSign) = 
    let addressee = msg.GetResult(CommandArguments.Addressee)
    let message = msg.GetResult(CommandArguments.Message)
    let addrMsg = "ADDRESSEE cannot be empty and must be 1 - 9 characters."
    let msgText = "MESSAGE TEXT must be less than 68 characters and cannot contain | ~"
    let msgNum = "MESSAGE NUMBER must be less than 10"
    let information =
        match CallSign.create addressee, Message.MessageText.create message, Message.MessageNumber.create String.Empty with
        | Some c, Some m, Some n -> {
                                        Addressee = c
                                        MessageText = m
                                        MessageNumber = Some n
                                    } |> Message.MessageFormat.Message 
                                    |> Information.Message
        | None, Some _, Some _ -> failwith addressee //TODO use a proper flow/pipeline with result type instead?
        | None, None, Some _ -> failwith (sprintf "%s %s" addrMsg msgText)
        | None, Some _, None -> failwith (sprintf "%s %s" addrMsg msgNum)
        | Some _, None, None -> failwith (sprintf "%s %s" msgText msgNum)
        | Some _, Some _, None -> failwith msgNum
        | Some _, None, Some _ -> failwith msgText
        | None, None, None -> failwith (sprintf "%s %s %s" addrMsg msgText msgNum)

    {
        Sender = sender
        Destination = destination
        Path = WIDEnN WIDE11
        Information = Some information
    } |> Ok
