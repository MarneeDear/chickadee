module PrintOutput

open Console
open chickadee.core
open chickadee.infrastructure.TNC2MONRepository
open chickadee.core.TNC2MON


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
    | PositionReport.PositionReportFormat.PositionReportWithoutTimeStampOrUltimeter p -> Console.complete "PositionReportWithoutTimeStampOrUltimeter"
                                                                                         printPos p
    | PositionReport.PositionReportFormat.PositionReportWithoutTimeStampWithMessaging p -> Console.complete "PositionReportWithoutTimeStampWithMessaging"
                                                                                           printPos p
    | PositionReport.PositionReportFormat.PositionReportWithTimestampNoMessaging p -> Console.complete "PositionReportWithTimestampNoMessaging"
                                                                                      printPos p
    | PositionReport.PositionReportFormat.PositionReportWithTimestampWithMessaging p -> Console.complete "PositionReportWithTimestampWithMessaging"
                                                                                        printPos p
//:KB2ICI-14:ack003
let printMessage (msg:Message.MessageFormat) =
    match msg with
    | Message.MessageFormat.Message m -> Console.complete "This is a Message"
                                         Console.ok (sprintf "ADDRESSEE: %A" m.Addressee)
                                         Console.ok (sprintf "MESSAGE: %A" m.MessageText)
                                         Console.ok (sprintf "NUMBER: %A" m.MessageNumber)
    | Message.MessageFormat.MessageAcknowledgement ma -> Console.complete "This is a message acknowledgement."
                                                         Console.ok (sprintf "NUMBER: %A" ma.MessageNumber)
    | Message.MessageFormat.MessageRejection mr -> Console.complete "This is a message rejection."
                                                   Console.ok (sprintf "NUMBER: %A" mr.MessageNumber)
    | Message.MessageFormat.Announcement a -> Console.complete "This is an announcement."
                                              Console.ok (sprintf "ANNOUNCEMENT ID: %A" a.AnnouncementId)
                                              Console.ok (sprintf "ANNOUNCEMENT TEXT: %A" a.AnnouncementText)
    | Message.MessageFormat.Bulletin b -> Console.complete "This is a bulletin."
                                          Console.ok (sprintf "BULLETIN ID: %A" b.BulletinId)
                                          Console.ok (sprintf "BULLETIN TEXT: %A" b.BulletinText)

let printPacket (packet:Result<Packet,string list>) =
    match packet with
    | Ok p -> Console.complete "Successfully parsed your frame. Here is what I got."
              Console.ok (sprintf "APRS FRAME: %s" (p.ToString()))                      
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
