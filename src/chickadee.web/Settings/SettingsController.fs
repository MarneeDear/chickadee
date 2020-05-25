namespace Settings

open Saturn
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Model
open chickadee.infrastructure.DireWolf
open chickadee.core.TNC2MON
open chickadee.core
open chickadee.core.Message
open chickadee.core.DataFormats
open Config
open System.IO
open Thoth.Json.Net

module Controller =

    let indexAction (ctx: HttpContext) = 
        let cnf = Controller.getConfig ctx
        let logger = ctx.GetLogger()

        task {
            try
                let! settingsJson = Helpers.getUserSettings ()
                let settings =
                    match settingsJson with
                    | Ok s -> s
                    | Error m -> logger.LogError m
                                 Helpers.emptyUserSettings ()
                return Views.index settings 
                
            with
            | ex -> logger.LogError ex.Message                    
                    return Views.index (Helpers.emptyUserSettings())
        }

    let addAction (ctx:HttpContext) =
        let cnf = Controller.getConfig ctx
        let logger = ctx.GetLogger()

        task {
            return Views.add ctx
        }

    let createAction (ctx:HttpContext) =
        let cnf = Controller.getConfig ctx
        let logger = ctx.GetLogger()
        task { 
            let! model = Controller.getModel<Helpers.UserSettings> ctx
            let objectModel = Encode.object
                                    [
                                        "CallSign", Encode.string model.CallSign
                                        "LocationLatitude", Encode.float model.LocationLatitude
                                        "LocationLongitude", Encode.float model.LocationLongitude
                                    ]
            let json = Encode.toString 2 objectModel
            do! File.WriteAllTextAsync("usersettings.json", json)
            Helpers.setSettingsSession ctx model
            return! Controller.redirect ctx "/settings"
        }

    let resource = controller {
        index indexAction
        add addAction
        create createAction
    }
