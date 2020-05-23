namespace PositionReports

open Saturn
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http

module Controller =

    let indexAction (ctx: HttpContext) = 
        task {
            return Views.index Seq.empty Seq.empty
        }

    let resource = controller {
        index indexAction
    }