namespace Settings

module Views = 
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine

    let index (settings:Helpers.UserSettings) =
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
                            a [_href "/settings/add"] [encodedText "Update Your User Settings"]
                        ]
                    ]
                ]
                h1 [_class "title has-background-info"; _style "padding: 10px 10px 10px 10px"] [ encodedText "Your User Settings" ]
            ]
            section [_class "section"] [
                div [_class "box"] [
                    label [_class "label"] [encodedText "Your Call Sign"]                    
                    div [_class "content has-text-info"] [
                        encodedText "This will be used as Sender part of created packets."
                    ]
                    div [_class "content"] [
                        encodedText settings.CallSign
                    ]
                ]
                div [_class "box"] [
                    label [_class "label"] [encodedText "Your Location Latitude in Decimal Format"]                    
                
                    div [_class "content has-text-info"] [
                        encodedText "This will be used as the center of the map and your default position for position reports."
                    ]
                    div [_class "content"] [
                        encodedText (string settings.LocationLatitude)
                    ]
                ]
                div [_class "box"] [
                    label [_class "label"] [encodedText "Your Location Longitude in Decimal Format"]                    
                    div [_class "content has-text-info"] [
                        encodedText "This will be used as the center of the map and your default position for position reports."
                    ]
                    div [_class "content"] [
                        encodedText (string settings.LocationLongitude)
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
                            a [_href "/settings"] [encodedText "Settings"]
                        ]

                    ]
                ]
                h1 [_class "title"] [encodedText "Update Your User Settings"]
                Helpers.protectedForm ctx [ _method "post"; _action "/settings"; _name "message"; _id "message"; ] [
                    div [_class "columns"] [                    
                        div [_class "column"] [
                            div [_id "settingsForm"] [
                                div [_class "field"] [
                                    label [_class "label"] [encodedText "Call Sign: This will be used as the Sender part of created packets."]
                                    div [_class "control"] [
                                        input [_class "input"; _name "CallSign"; _type "text"; _placeholder "Call Sign (KG7SIO)"; _maxlength "9"] 
                                    ]
                                ]
                                div [_class "field"] [
                                    label [_class "label"] [encodedText "Location Latitude in Decimal Format: This will be used as the center of the map and your default position for position reports."]
                                    div [_class "control"] [
                                        input [_class "input"; _name "LocationLatitude"; _type "text"; _placeholder "Where are you?"; _maxlength "15"]
                                    ]
                                ]
                                div [_class "field"] [
                                    label [_class "label"] [encodedText "Location Longitude in Decimal Format: This will be used as the center of the map and your default position for position reports."]
                                    div [_class "control"] [
                                        input [_class "input"; _name "LocationLongitude"; _type "text"; _placeholder "Where are you?"; _maxlength "15"]
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
