module FrameParser

open System
open chickadee.core.TNC2MON
open chickadee.core
open chickadee.core.TNC2MonActivePatterns
open chickadee.core.APRSDataFormats
open chickadee.core.DataFormats.DataFormatType
open chickadee.core.DataFormats.PositionReportActivePatterns
open chickadee.core.PositionReport
open chickadee.core.Message
open chickadee.core.DataFormats.MessageActivePatterns

let parseFrame (frame:string) : Result<Packet, string list> =
    let errorsList = []

    let sender = 
        match frame with
        | Sender s -> 
                       match CallSign.create s with
                       | Some c -> Ok c
                       | None -> Error "Could not create a call sign from the destination."
        | _ -> Error "Could not parse Sender"

    let destination =
        match frame with
        | Destination d -> 
                            match CallSign.create d with
                            | Some c -> Ok c
                            | None -> Error "Could not create a call sign from the destination."
        | _ -> Error "Could not parse the destination"

    let path =
        match frame with
        | Path p -> Ok (WIDEnN WIDE11) //TODO handle paths
        | _ -> Error "Could not parse Path"

    let composeMessage (msgText:MessageText) (info:string) =
        let addressee =
            match info with
            | Addressee a -> Ok a
            | _ -> Error "Could not parse addressee."

        //let message =
        //    match info with
        //    | Message m -> Ok m
        //    | _ -> Error "Could not parse message."

        //TODO message Number is optional
        //let messageNum =
        //    match info with
        //    | MessageNumber n -> Ok n
        //    | _ -> Error "Could not parse message number."

        match addressee with
        | Ok a -> let num = (|MessageNumber|_|) info
                  {
                                    Addressee = a
                                    MessageText = msgText
                                    MessageNumber = num
                  } |> MessageFormat.Message |> TNC2MON.Information.Message |> Ok
        //| Ok _ -> Error (errorsList |> List.append [m])
        //| Ok _, Error m, Ok _  -> Error (errorsList |> List.append [m])
        //| Ok _, Error m -> Error (errorsList |> List.append [m])
        | Error m -> Error (errorsList |> List.append [m])
        //| Error m1, Error m2 -> Error (errorsList |> List.append [m1] |> List.append [m2])
        //| Error m, Error m2, Ok _ -> Error (errorsList |> List.append [m] |> List.append [m2])
        //| Error m1, Ok _, Error m2 -> Error (errorsList |> List.append [m1] |> List.append [m2])
        //| Error m1, Error m2, Error m3 -> Error (errorsList |> List.append [m1] |> List.append [m2] |> List.append [m3])

        //match addressee, message, messageNum with
        //| Ok a, Ok m, Ok n ->  {
        //                            Addressee = a
        //                            MessageText = m
        //                            MessageNumber = n
        //                        } |> MessageFormat.Message |> TNC2MON.Information.Message |> Ok
        //| Ok _, Ok _, Error m -> Error (errorsList |> List.append [m])
        //| Ok _, Error m, Ok _  -> Error (errorsList |> List.append [m])
        //| Ok _, Error m, Error sm -> Error (errorsList |> List.append [m] |> List.append [sm])
        //| Error m, Ok _, Ok _ -> Error (errorsList |> List.append [m])
        //| Error m, Error m2, Ok _ -> Error (errorsList |> List.append [m] |> List.append [m2])
        //| Error m1, Ok _, Error m2 -> Error (errorsList |> List.append [m1] |> List.append [m2])
        //| Error m1, Error m2, Error m3 -> Error (errorsList |> List.append [m1] |> List.append [m2] |> List.append [m3])

    let composeAck (msgNum:MessageNumber) (info:string) =
        let addressee =
            match info with
            | Addressee a -> Ok a
            | _ -> Error "Could not parse addressee."

        match addressee with
        | Ok a -> let msgAck : MessageAcknowledgement =        
                        {
                            Addressee = a
                            MessageNumber = msgNum
                        } 
                  msgAck |> MessageFormat.MessageAcknowledgement |> TNC2MON.Information.Message |> Ok
        | Error m -> errorsList |> List.append [m] |> Error

    let composeRej (msgNum:MessageNumber) (info:string) =
        let addressee =
            match info with
            | Addressee a -> Ok a
            | _ -> Error "Could not parse addressee."

        match addressee with
        | Ok a -> let msgRej : MessageRejection =        
                        {
                            Addressee = a
                            MessageNumber = msgNum
                        } 
                  msgRej |> MessageFormat.MessageRejection |> TNC2MON.Information.Message |> Ok
        | Error m -> errorsList |> List.append [m] |> Error

    let parseMessageFormat (info:string) =
        //let addressee =
        //    match info with
        //    | Addressee a -> Ok a
        //    | _ -> Error "Could not parse addressee."

        let message =
            match info with
            | Message m -> Ok m
            | _ -> Error "Could not parse message."

        let messageAck =
            match info with
            | MessageAcknowledgementNumber ma -> Ok ma
            | _ -> Error "Could not parse message acknowledgement."

        let messageRej =
            match info with
            | MessageRejectionNumber mj -> Ok mj
            | _ -> Error "Could not parse message rejection."

        match message, messageAck, messageRej with
        | Ok m, Error _, Error _ -> composeMessage m info
        | Error _, Ok m, Error _ -> composeAck m info
        | Error _, Error _, Ok m -> composeRej m info
        | _, _, _ -> errorsList |> List.append ["Could not parse message format. This is not a message, an acknowledgement, or a rejection."] |> Error

    let parsePositionReport (info:string) =
        let lat =
            match info with
            | Latitude l -> Ok (FormattedLatitude.check l)
            | _ -> Error "Could not parse latitude"

        let lon =
            match info with
            | Longitude l -> Ok (FormattedLongitude.check l)
            | _ -> Error "Could not parse longitude"

        let sym =
            match info with
            | Symbol s -> Ok s
            | _ -> Error "Could not parse symbol"

        let comment symChar = 
            match info with
            | Comment symChar c -> Ok c
            | _ -> Error "Could not parse comment"

        match lat, lon, sym with
        | Ok lat, Ok lon, Ok sym -> let symChar, _ = sym.ToCode()
                                    match comment symChar with
                                    | Ok c ->   
                                                {
                                                    Position = {Latitude = lat; Longitude = lon}
                                                    Symbol = sym
                                                    TimeStamp = None
                                                    Comment = PositionReportComment.create c
                                                } |> PositionReport.PositionReportFormat.PositionReportWithoutTimeStampWithMessaging //todo support all types
                                                  |> Information.PositionReport |> Ok
                                    | Error m -> Error (errorsList |> List.append [m])
        | Ok _, Ok _, Error m -> Error (errorsList |> List.append [m])
        | Ok _, Error m, Ok _  -> Error (errorsList |> List.append [m])
        | Ok _, Error m, Error sm -> Error (errorsList |> List.append [m] |> List.append [sm])
        | Error m, Ok _, Ok _ -> Error (errorsList |> List.append [m])
        | Error m, Error m2, Ok _ -> Error (errorsList |> List.append [m] |> List.append [m2])
        | Error m1, Ok _, Error m2 -> Error (errorsList |> List.append [m1] |> List.append [m2])
        | Error m1, Error m2, Error m3 -> Error (errorsList |> List.append [m1] |> List.append [m2] |> List.append [m3])

    let information =
        match frame with
        | Information i -> 
                            match (|FormatType|_|) i with
                            | Some r -> match r with
                                        | DataFormat.PostionReport p -> parsePositionReport i //(i.Substring(1))
                                        | DataFormat.Message -> parseMessageFormat (i.Substring(1))
                                        | _ -> Error ([sprintf "Could not parse frame but got this format [%s]" (r.ToString())])
                            | None -> Error (["Did not recognize data format type"])
        | _ -> Error (errorsList |> List.append ["Could not parse Information"])

    match sender, destination, path, information with
    | Ok s, Ok d, Ok p, Ok i -> {
                                    Sender = s
                                    Destination = d
                                    Path = p
                                    Information = Some i
                                } |> Ok
    | Ok _, Ok _, Ok _, Error m -> Error m
    | Ok _, Ok _, Error m, Ok _ -> Error [m]
    | Ok _, Error m, Ok _, Ok _ -> Error [m]
    | Error m, Ok _, Ok _, Ok _ -> Error [m]
    | Ok _, Ok _, Error m1, Error m2 -> Error (m2 |> List.append [m1])
    | Ok _, Error m1, Error m2, Error m3 -> Error (m3 |> List.append [m1] |> List.append [m2])
    | Error m1, Error m2, Error m3, Error m4 -> Error (m4 |> List.append[m1] |> List.append [m2] |> List.append [m3])
    | Error m1, Ok _, Ok _, Error m2 -> Error (m2 |> List.append [m1])
    | Error m1, Ok _, Error m2, Ok _ -> Error ([m1] |> List.append [m2])
    | Error m1, Error m2, Ok _, Ok _ -> Error ([m1] |> List.append [m2])
    | Ok _, Error m1, Error m2, Ok _ -> Error([m1] |> List.append [m2])
    | Ok _, Error m1, Ok _, Error m2 -> Error(m2 |> List.append [m1])
    | Error m1, Error m2, Error m3, Ok _ -> Error ([m1] |> List.append [m2] |> List.append [m3])
    | Error m1, Ok _, Error m3, Error m4 -> Error (m4 |> List.append [m1] |> List.append [m3])
    | Error m1, Error m2, Ok _, Error m4 -> Error (m4 |> List.append [m1] |> List.append [m2])
    //TODO this is crazy. what else can I do?

    ////TODO use the parsed information field
    //let informationFormat : Information =
    //    let msg : Message.Message = 
    //        {
    //            Addressee = (CallSign.create "KG7SIO").Value
    //            MessageText = (Message.MessageText.create "The spice must flow.").Value
    //            MessageNumber = (Message.MessageNumber.create "1").Value
    //        }
    //    msg |> Message.MessageFormat.Message |> TNC2MON.Information.Message //|> TNC2MON.Information    
    ////TODO use the parsed information field
    //{
    //    Sender = (CallSign.create "KG7SIO").Value
    //    Destination = (CallSign.create "W7HND").Value
    //    Path = WIDEnN WIDE11
    //    Information = Some informationFormat
    //}
