namespace Messages

open Saturn
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http

module Controller =

    let indexAction (ctx: HttpContext) = 
        task {
            let messagesList =      
                [
                    
                ]
            return Views.index messagesList
        }

    let resource = controller {
        index indexAction
    }
