namespace Map

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine
    (*
        Custom Symbols
        https://github.com/Azure-Samples/AzureMapsCodeSamples/blob/master/AzureMapsCodeSamples/Symbol%20Layer/Custom%20Symbol%20Image%20Icon.html
    *)
    let index (otp:string) (clientId:string) =
        let content = [
            section [_class "section"] [
                div [] [
                    input [_type "hidden"; _id "otp"; _value otp]
                    input [_type "hidden"; _id "clientId"; _value clientId]
                ]
                nav [_class "breadcrumb"] [
                    ul [] [
                        li [] [
                            a [_href "/"] [encodedText "Home"]
                        ]
                    ]
                ]
                div [_id "map"; _style "min-width: 290px; width: 100%; height: 600px"] []
            ]
            link [_rel "stylesheet"; _href "https://atlas.microsoft.com/sdk/javascript/mapcontrol/2/atlas.min.css"]
            script [_src "https://atlas.microsoft.com/sdk/javascript/mapcontrol/2/atlas.min.js"] []
            script [_src "/map.js"] []
            
        ]
        MapApp.layout content
