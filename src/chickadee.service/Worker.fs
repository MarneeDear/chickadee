namespace chickadee.service

module Workers =

    open System
    open System.Threading
    open System.Threading.Tasks
    open Microsoft.Extensions.Configuration
    open Microsoft.Extensions.Hosting
    open Microsoft.Extensions.Logging
    open FSharp.Control.Tasks.ContextInsensitive
    open chickadee.infrastructure.DireWolf
    open chickadee.core

    let settings (config:IConfiguration) : Settings.WorkerOptions = //configuration.GetSection("DireWolf").Get<Settings.DireWolfSettings>()
        let cnf = config.GetSection("DireWolf")
        {
            TransmitFilePath = cnf.GetValue("ReadInterval")
            TransmitFileNameSuffix = cnf.GetValue("TransmitFileNameSuffix")
            ReceivedFilePath = cnf.GetValue("ReceivedFilePath")
            ReadInterval = int (cnf.GetValue("ReadInterval"))
            WriteInterval = int (cnf.GetValue("WriteInterval"))
            //Sqlite = config.GetSection("Database").GetValue("Sqlite")
            Sqlite = ""
            Environment = config.GetValue("Environment") //cnf.GetValue("Environment")
        }

    type ReadWorker(logger : ILogger<ReadWorker>, configuration : IConfiguration ) =
        inherit BackgroundService()

        override _.ExecuteAsync(stoppingToken : CancellationToken) =
            
            let st = settings configuration
            let logFound = sprintf "READING [%s]"
            let separator = "------------------------------------------------------------"
            task {
                //do! Console.Out.WriteLineAsync(sprintf "ENVIRONMENT [%s]" st.Environment)
                //do! Console.Out.WriteLineAsync(sprintf "ENVIRONMENT [%s]" (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
                while not stoppingToken.IsCancellationRequested do                    
                    let files = System.IO.Directory.GetFiles(st.ReceivedFilePath)
                    if files.Length = 0 then
                        logger.LogInformation(separator)
                        logger.LogInformation("No files found.")
                    for file in files do
                        logger.LogInformation separator
                        logger.LogInformation file
                        KissUtil.getRecords st.ReceivedFilePath None
                        |> Array.iter (fun r -> logger.LogInformation (logFound r))
                        let fileInfo = new System.IO.FileInfo(file)
                        
                        let logResults (r:Result<int, exn>) =
                            match r with
                            | Ok _ -> ()
                            | Error e -> logger.LogInformation e.Message

                        KissUtil.saveReveivedRawRecordss st.Sqlite st.ReceivedFilePath (Some fileInfo.Name)
                        |> Array.iter logResults                        
                        
                        System.IO.File.Move (file, (sprintf "%s/processed/%s" st.ReceivedFilePath fileInfo.Name))
                    do! Task.Delay(st.ReadInterval, stoppingToken) 
            } :> Task

    type WriteWorker(logger : ILogger<ReadWorker>, configuration : IConfiguration ) =
        inherit BackgroundService()

        let st = settings configuration

        override _.ExecuteAsync(stoppingToken : CancellationToken) =
            task {
                while not stoppingToken.IsCancellationRequested do
                    //do! Console.Out.WriteLineAsync("HELLO WRITE ME")
                    //do! Task.Delay(st.WriteInterval, stoppingToken)
                    do! Task.Delay(st.WriteInterval, stoppingToken)
                            
            } :> Task
