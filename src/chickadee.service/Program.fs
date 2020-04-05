// Learn more about F# at http://fsharp.org
module chickadee.service.App

open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore

(*
https://stackoverflow.com/questions/58183920/how-to-setup-app-settings-in-a-net-core-3-worker-service
*)

[<EntryPoint>]
let main argv =
    printfn "The Chickadee service is starting ... ."
    Host.CreateDefaultBuilder(argv)
        .ConfigureAppConfiguration(fun hostingContext config ->
            config.AddEnvironmentVariables() |> ignore
        )
        .ConfigureServices(fun hostContext services -> 
            services.AddHostedService<chickadee.service.Workers.ReadWorker>() |> ignore
            services.AddHostedService<chickadee.service.Workers.WriteWorker>() |> ignore
        )
        .Build().Run()
    
    0 // return an integer exit code
