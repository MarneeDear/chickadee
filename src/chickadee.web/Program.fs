module Server

open Saturn
open Config
open Serilog
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open System

(*
https://github.com/serilog/serilog-settings-configuration
*)
let configSerilog (builder:ILoggingBuilder) =
    builder.SetMinimumLevel(LogLevel.Information) |> ignore
    let config = builder.Services.BuildServiceProvider().GetService<IConfiguration>()
    let loggingConfig = new LoggerConfiguration()
    loggingConfig.ReadFrom.Configuration(config) |> ignore
    builder.AddSerilog(loggingConfig.CreateLogger()) |> ignore

let setupConfig (config:IConfiguration)  =
    {
        connectionString = config.["Database:Sqlite"]
        tokenApiKey = config.["TokenAPIKey"]
        clientId = config.["AzureMaps:ClientID"]
        otpSeed = config.["OtpSeed"]
    }

let configureSession (services : IServiceCollection) =
    services.AddSession(fun opts ->
        opts.IdleTimeout <- TimeSpan.FromMinutes(30.0)
    ) |> ignore
    services

let configureSerialization (services:IServiceCollection) =
    services.AddSingleton<Giraffe.Serialization.Json.IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer())

let endpointPipe = pipeline {
    plug head
    plug requestId
}

let app = application {
    pipe_through endpointPipe
    use_antiforgery
    service_config configureSerialization
    logging configSerilog
    use_config setupConfig
    error_handler (fun ex logger -> 
                                    logger.LogCritical(ex.Message)
                                    pipeline { render_html (InternalError.layout ex) })
    use_router Router.appRouter
    url "https://chickadee.local/"
    service_config configureSession
    memory_cache
    use_static "static"
    use_gzip
    
}

[<EntryPoint>]
let main _ =
    printfn "Working directory - %s" (System.IO.Directory.GetCurrentDirectory())
    run app
    0 // return an integer exit code