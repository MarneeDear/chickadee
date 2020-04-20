namespace Messages

open Saturn
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe
open Model

module Controller =

    let indexAction (ctx: HttpContext) = 
        task {
            let messagesList =      
                [
                    
                ]
            return Views.index messagesList
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
            let! model = Controller.getModel<Message> ctx
            //TODO add to database and allow background job to handle writing to DireWolf
            return! Controller.redirect ctx "/messages"
        }

    let resource = controller {
        index indexAction
        add addAction
        create createAction
    }
