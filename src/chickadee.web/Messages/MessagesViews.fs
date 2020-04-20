namespace Messages

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine

    let index (v:string list) = 
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
                        button [_class "button"] [
                            a [_href "/messages/add"] [encodedText "Create Message"]
                        ]
                    ]
                ]                
                div [ _class "hero is-warning" ] [ 
                    div [ _class "hero-body"] [ 
                        p [ _class "title" ] [encodedText "Refresh to see all of the latest APRS messages"]
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
                                        input [_class "input"; _name "MessageNumber"; _type "text"; _placeholder "00001"; _maxlength "5"]
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