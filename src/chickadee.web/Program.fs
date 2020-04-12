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
    //Should be done in the appsettings Loggin Sinks and AWS Serilog
    //loggingConfig.MinimumLevel.Information |> ignore
    //loggingConfig.MinimumLevel.Override("Microsoft", LogEventLevel.) |> ignore
    //loggingConfig.Enrich.FromLogContext() |> ignore

    //match config.["Logging:Sink"] with
    ////| "RollingFile" -> loggingConfig.WriteTo.RollingFile("D:\logs\log-{Date}.txt") |> ignore//.CreateLogger();
    //| "File" -> loggingConfig.WriteTo.File("D:\\log.log", rollingInterval = RollingInterval.Day) |> ignore
    //| "Console" -> loggingConfig.WriteTo.Console() |> ignore
    //| _ -> loggingConfig.WriteTo.Console() |> ignore

    builder.AddSerilog(loggingConfig.CreateLogger()) |> ignore


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
    use_config (fun _ -> {connectionString = "DataSource=..\..\database\database.sqlite;"} ) //TODO: Set development time configuration
}

[<EntryPoint>]
let main _ =
    printfn "Working directory - %s" (System.IO.Directory.GetCurrentDirectory())
    run app
    0 // return an integer exit code