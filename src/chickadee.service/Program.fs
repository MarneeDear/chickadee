// Learn more about F# at http://fsharp.org
module chickadee.service.App

open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

[<EntryPoint>]
let main argv =
    printfn "The Chickadee service is starting ... ."
    let host =
        Host.CreateDefaultBuilder(argv)
            .ConfigureServices(fun hostContext services -> 
                services.AddHostedService<chickadee.service.Workers.ReadWorker>() |> ignore
                services.AddHostedService<chickadee.service.Workers.WriteWorker>() |> ignore
            )
            .Build().Run()
    
    0 // return an integer exit code
