module MapData

open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Saturn

type MapData =
    {
        CallSign : string
        Image : string
        Location : float list
    }

module Controller =

    let indexAction (ctx:HttpContext) =
    //TODO map a symbol to an image
    //REname the images?
        task {
            let imgPth = "/image/map/aprs-symbols/primary/"
            let mapData = [ {
                CallSign = "KG7SIO-1"
                Image = sprintf "%s%s" imgPth "car.png"
                Location = [-110.911789; 32.253460]
            };
            {
                CallSign = "KG7SIO-2"
                Image = sprintf "%s%s" imgPth "ambulance.png"
                Location = [-111.166618; 32.296738]
            };
            {
                CallSign = "KG7SIO-3"
                Image = sprintf "%s%s" imgPth "fire.png"
                Location = [-110.788124; 32.443417]
            }]
            return! ControllerHelpers.Controller.json ctx mapData
        }

    let resource = controller {
        index indexAction
    }
