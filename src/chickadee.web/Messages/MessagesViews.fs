namespace Messages

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine
    open Model

    let index (rxs:ReceivedMessage seq) (txs:TransmittedMessage seq) = 
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
                            a [_href "/messages/add"] [encodedText "Create Message"]
                        ]
                    ]
                ]
                h1 [_class "title has-background-warning"; _style "padding: 10px 10px 10px 10px"] [encodedText "Refresh to see all of the latest APRS messages"]
            ]
            section [_class "section"] [                
                h1 [_class "title"] [encodedText "Received Messages"]
                table [_class "table"] [
                    thead [] [
                        th [] [encodedText "Date Created"]
                        th [] [encodedText "Raw Packet"]
                        th [] [encodedText "Addressee"]
                        th [] [encodedText "Message Text"]
                        th [] [encodedText "Message Number"]
                        th [] [encodedText "Error"]
                    ]
                    tbody [] [
                        for rx in rxs do
                            tr [] [
                                td [] [encodedText rx.DateCreated]
                                td [_style "word-wrap: break-word;"] [encodedText rx.RawPacket]
                                td [] [encodedText rx.Addressee]
                                td [_style "word-wrap: break-word; "] [encodedText rx.MessageText]
                                td [] [encodedText rx.MessageNumber]
                                td [_style "word-wrap: break-word; "] [encodedText rx.Error]
                            ]
                    ]
                ]
            ]
            section [_class "section"] [
                h1 [_class "title"] [encodedText "Transmited Messages"]
                table [_class "table"] [
                    thead [] [
                        th [] [encodedText "Date Created"]
                        th [_style "word-wrap: break-word; "] [encodedText "Raw Packet"]
                        th [] [encodedText "Addressee"]
                        th [_style "word-wrap: break-word; "] [encodedText "Message Text"]
                        th [] [encodedText "Message Number"]
                        th [_style "word-wrap: break-word; "] [encodedText "Error"]
                        th [] [encodedText "Transmitted?"]
                    ]
                    tbody [] [
                        for tx in txs do
                            tr [] [
                                td [] [encodedText tx.DateCreated]
                                td [] [encodedText tx.RawPacket]
                                td [] [encodedText tx.Addressee]
                                td [] [encodedText tx.MessageText]
                                td [] [encodedText tx.MessageNumber]
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
                            a [_href "/messages"] [encodedText "All Sent and Received Messages"]
                        ]
                    ]
                ]
                h1 [_class "title"] [encodedText "Create Messages"]
                Helpers.protectedForm ctx [ _method "post"; _action "/messages"; _name "message"; _id "message"; ] [
                    div [_class "columns"] [                    
                        div [_class "column"] [
                            div [_id "messageForm"] [
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