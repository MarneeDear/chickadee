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
            let mapData = [ {
                CallSign = "KG7SIO-1"
                Image = "/image/map/aprs-symbols/aprs_small/image_part_001.png"
                Location = [-110.911789; 32.253460]
            };
            {
                CallSign = "KG7SIO-2"
                Image = "/image/map/aprs-symbols/aprs_small/image_part_002.png"
                Location = [-111.166618; 32.296738]
            };
            {
                CallSign = "KG7SIO-3"
                Image = "/image/map/aprs-symbols/aprs_small/image_part_003.png"
                Location = [-110.788124; 32.443417]
            }]
            return! ControllerHelpers.Controller.json ctx mapData
        }

    let resource = controller {
        index indexAction
    }
