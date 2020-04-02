// Learn more about F# at http://fsharp.org
module chickadee.service.App

open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration

[<EntryPoint>]
let main argv =
    printfn "The Chickadee service is starting ... ."
    Host.CreateDefaultBuilder(argv)
        .ConfigureServices(fun hostContext services -> 
            //hostContext.HostingEnvironment.EnvironmentName <- "Development"

            services.AddHostedService<chickadee.service.Workers.ReadWorker>() |> ignore
            services.AddHostedService<chickadee.service.Workers.WriteWorker>() |> ignore
            //()
            //let env = hostContext.HostingEnvironment
            //if (env.IsDevelopment()) then
            //    hostContext.Configuration()
        )
        .ConfigureAppConfiguration(fun hostingContext config ->
            config.AddEnvironmentVariables() |> ignore
        )
        .Build().Run()
    
    0 // return an integer exit code
