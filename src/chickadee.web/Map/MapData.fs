namespace AprsMap

open chickadee.core.PositionReport
open chickadee.infrastructure.DireWolf

module MapData =

    open FSharp.Control.Tasks.ContextInsensitive
    open Microsoft.AspNetCore.Http
    open Giraffe
    open Saturn
    open chickadee.core
    open chickadee.infrastructure
    open Microsoft.Extensions.Logging
    open Models
    open Config

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

            let mapSymbolToIcon (symbol:SymbolCode) =
                (icons |> List.find (fun s -> s.Symbol = symbol)).Icon

            let mapToMapData sender (posRpt:PositionReport) =
                let lat, lon = TNC2MONRepository.convertPoitionToCoordinates posRpt
                {
                    CallSign = sender
                    Image = sprintf "%s%s" imgPth (mapSymbolToIcon posRpt.Symbol)
                    Location = [lon; lat]
                }

            //GET sender part
            let sender rx = 
                match rx with
                | TNC2MonActivePatterns.Sender s -> s
                | _ -> System.String.Empty

            let positionReport rx =
                match TNC2MONRepository.convertFrameToAPRSData rx with
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
                let! rxResult = KissUtil.getReceivedFrames cnf.connectionString (Some "Position Report")
                 
                let mapData rx = 
                    match positionReport rx with
                    | Ok posRpt -> (mapToMapData (sender rx) posRpt) 
                    | Error m -> logger.LogError(m)
                                 {
                                    CallSign = System.String.Empty
                                    Image = System.String.Empty
                                    Location = []
                                }

                let result =
                    match rxResult with
                    | Ok rxs -> rxs |> Seq.map (fun r -> r.raw_packet) |> Seq.map mapData |> Seq.toList
                    | Error m -> logger.LogError(m.Message)
                                 []
                             
                return! ControllerHelpers.Controller.json ctx (testmapData |> List.append result)
            }

        let resource = controller {
            index indexAction
        }
