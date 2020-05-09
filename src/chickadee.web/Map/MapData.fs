namespace AprsMap

open chickadee.core.PositionReport

module MapData =

    open FSharp.Control.Tasks.ContextInsensitive
    open Microsoft.AspNetCore.Http
    open Giraffe
    open Saturn
    open chickadee.core
    open chickadee.infrastructure
    open Microsoft.Extensions.Logging
    open Models

    type MapData =
        {
            CallSign : string
            Image : string
            Location : float list
        }

    module Controller =

        let indexAction (ctx:HttpContext) =
            let cnf = Controller.getConfig ctx
            let logger = ctx.GetLogger()
            let test = "KG7SIO-7>APRD15,WIDE1-1:=3206.86N/11056.35Wb,b>,lah:blah /fishcakes"
            let imgPth = "/image/map/aprs-symbols/primary/"

            let testmapData =
                [
                    {
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
                        Location = [-110.98173; 32.339259]
                    }
                ]

            //TODO map a symbol to an image
            //REname the images?
            //Get from database and convert position report symbols to an icon
            //TODO convert position to coordinates azure maps can understand

            let mapSymbolToIcon (symbol:SymbolCode) =
                (icons |> List.find (fun s -> s.Symbol = symbol)).Icon

            let mapToMapData sender (posRpt:PositionReport) =
                let lat, lon = TNC2MONRepository.convertPoitionToCoordinates posRpt
                {
                    CallSign = sender
                    Image = sprintf "%s%s" imgPth (mapSymbolToIcon posRpt.Symbol)
                    Location = [lon; lat]
                } |> Ok

            //GET sender part
            let sender = 
                match test with
                | TNC2MonActivePatterns.Sender s -> s
                | _ -> System.String.Empty

            let positionReport =
                match TNC2MONRepository.convertFrameToAPRSData test with
                | Ok info -> match info with
                             | TNC2MON.Information.PositionReport rpt -> match rpt with
                                                                         | PositionReportFormat.PositionReportWithoutTimeStampOrUltimeter p -> Ok p
                                                                         | PositionReportFormat.PositionReportWithoutTimeStampWithMessaging p -> Ok p
                                                                         | PositionReportFormat.PositionReportWithTimestampNoMessaging p -> Ok p
                                                                         | PositionReportFormat.PositionReportWithTimestampWithMessaging p -> Ok p
                             | _ -> Error "Not a position report"
                            //mapToMapData sender rpt
                | Error m -> Error m
            
            

            task {
                let mapData = 
                    match positionReport |> Result.bind (mapToMapData sender) with
                    | Ok data -> testmapData |> List.append [data]
                    | Error m -> logger.LogError(m)
                                 testmapData
                    //[ 
                    ////{
                    ////    CallSign = sender
                    ////    Image = sprintf "%s%s" imgPth "car.png"
                    ////    Location = [-110.911789; 32.253460]
                    ////};
                    //]
                return! ControllerHelpers.Controller.json ctx mapData
            }

        let resource = controller {
            index indexAction
        }
