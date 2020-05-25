namespace Index

open Saturn
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Config

module Controller =
    let indexAction (ctx: HttpContext) =
        let cnf = Controller.getConfig ctx
        let logger = ctx.GetLogger()

        task {
            let! json = Helpers.getUserSettings() 
            match json with
            | Ok s -> return View.index (Some s)
            | Error m -> return View.index None
        }
        

    let resource = controller {
        index indexAction
    }


