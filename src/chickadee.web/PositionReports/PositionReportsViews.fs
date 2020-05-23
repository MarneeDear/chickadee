namespace PositionReports

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine
    open Model

    let index (rxs:PositionReportReceived seq) (txs:PositionReportTransmitted seq) = 
        let content = [
            section [_class "section"] [
                nav [_class "breadcrumb"] [
                    ul [] [
                        li [] [
                            a [_href "/"] [encodedText "Home"]
                        ]
                    ]
                ]
                div [_class "field"] [
                    div [_class "control"] [
                        button [_class "button has-background-danger"] [
                            a [_href "/position_reports/add"] [encodedText "Create Position Report"]
                        ]
                    ]
                ]
                h1 [_class "title has-background-warning"; _style "padding: 10px 10px 10px 10px"] [encodedText "Refresh to see all of the latest APRS position reports"]
            ]
            section [_class "section"] [                
                h1 [_class "title"] [encodedText "Received Position Reports"]
                table [_class "table"] [
                    thead [] [
                        th [] [encodedText "Date Created"]
                        th [] [encodedText "Raw Packet"]
                        th [] [encodedText "Report Type"]
                        th [] [encodedText "Raw Position"]
                        th [] [encodedText "Latitude"]
                        th [] [encodedText "Longitude"]
                        th [] [encodedText "Symbol"]
                        th [] [encodedText "Time Stamp"]
                        th [] [encodedText "Comment"]
                        th [] [encodedText "Error"]
                    ]
                    tbody [] [
                        for rx in rxs do
                            tr [] [
                                td [] [encodedText rx.DateCreated]
                                td [_style "word-wrap: break-word;"] [encodedText rx.RawPacket]
                                td [] [encodedText rx.PositionReport.ReportType]
                                td [_style "word-wrap: break-word; "] [encodedText rx.RawPosition]
                                td [] [encodedText rx.PositionReport.Latitude]
                                td [] [encodedText rx.PositionReport.Longitude]
                                td [] [
                                        img [_src rx.PositionReport.SymbolImage]
                                      ]
                                td [] [encodedText rx.PositionReport.TimeStamp]
                                td [] [encodedText rx.PositionReport.Comment]
                                td [_style "word-wrap: break-word; "] [encodedText rx.Error]
                            ]
                    ]
                ]
            ]
            section [_class "section"] [                
                h1 [_class "title"] [encodedText "Transmitted Position Reports"]
                table [_class "table"] [
                    thead [] [
                        th [] [encodedText "Date Created"]
                        th [] [encodedText "Raw Packet"]
                        th [] [encodedText "Report Type"]
                        th [] [encodedText "Raw Position"]
                        th [] [encodedText "Latitude"]
                        th [] [encodedText "Longitude"]
                        th [] [encodedText "Symbol"]
                        th [] [encodedText "Time Stamp"]
                        th [] [encodedText "Comment"]
                        th [] [encodedText "Error"]
                        th [] [encodedText "Transmitted?"]
                    ]
                    tbody [] [
                        for tx in txs do
                            tr [] [
                                td [] [encodedText tx.DateCreated]
                                td [_style "word-wrap: break-word;"] [encodedText tx.RawPacket]
                                td [] [encodedText tx.PositionReport.ReportType]
                                td [_style "word-wrap: break-word; "] [encodedText tx.RawPosition]
                                td [] [encodedText tx.PositionReport.Latitude]
                                td [] [encodedText tx.PositionReport.Longitude]
                                td [] [
                                        img [_src tx.PositionReport.SymbolImage]
                                      ]
                                td [] [encodedText tx.PositionReport.TimeStamp]
                                td [] [encodedText tx.PositionReport.Comment]
                                td [_style "word-wrap: break-word; "] [encodedText tx.Error]
                                td [] [encodedText (string tx.Transmitted)]
                            ]
                    ]
                ]
            ]
        ]
        App.layout content

    let add (ctx:HttpContext) =
        let content = [
            section [_class "section"] [
                nav [_class "breadcrumb"] [
                    ul [] [
                        li [] [
                            a [_href "/"] [encodedText "Home"]
                        ]
                        li [] [
                            a [_href "/position_report"] [encodedText "All Sent and Received Position Reports"]
                        ]
                    ]
                ]
                h1 [_class "title"] [encodedText "Create Position Reports"]
                Helpers.protectedForm ctx [ _method "post"; _action "/position_report"; _name "positionReport"; _id "positionReport"; ] [
                    div [_class "columns"] [                    
                        div [_class "column"] [
                            div [_id "positionReportForm"] [
                                div [_class "field"] [
                                    label [_class "label"] [encodedText "Addressee"]
                                    div [_class "control"] [
                                        input [_class "input"; _name "Addressee"; _type "text"; _placeholder "Call Sign (KG7SIO)"; _maxlength "9"] 
                                    ]
                                ]
                                div [_class "field"] [
                                    label [_class "label"] [encodedText "Message Text"]
                                    div [_class "control"] [
                                        input [_class "input"; _name "MessageText"; _type "text"; _placeholder "What do you want to say?"; _maxlength "67"]
                                    ]
                                ]
                                div [_class "field"] [
                                    label [_class "label"] [encodedText "Message Number"]
                                    div [_class "control"] [
                                        input [_class "input"; _name "MessageNumber"; _type "text"; _placeholder "99999"; _maxlength "5"]
                                    ]
                                ]
                            ]
                        ]
                    ]
                    button [_class "button"] [encodedText "Submit"]
                ]
            ]
        ]
        App.layout content