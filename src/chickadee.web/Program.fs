module Server

open Saturn
open Config
open Serilog
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration

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
    }

let endpointPipe = pipeline {
    plug head
    plug requestId
}

let app = application {
    pipe_through endpointPipe
    logging configSerilog
    error_handler (fun ex logger -> 
                                    logger.LogCritical(ex.Message)
                                    pipeline { render_html (InternalError.layout ex) })
    use_router Router.appRouter
    url "http://chickadee.local:8085/"
    memory_cache
    use_static "static"
    use_gzip
    use_config setupConfig
}

[<EntryPoint>]
let main _ =
    printfn "Working directory - %s" (System.IO.Directory.GetCurrentDirectory())
    run app
    0 // return an integer exit code