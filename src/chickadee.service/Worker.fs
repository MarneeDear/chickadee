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

    let getSettings (config:IConfiguration) : Settings.WorkerOptions = 
        let cnf = config.GetSection("DireWolf")
        {
            TransmitFilePath = cnf.GetValue("TransmitFilePath")
            TransmitFileNameSuffix = cnf.GetValue("TransmitFileNameSuffix")
            ReceivedFilePath = cnf.GetValue("ReceivedFilePath")
            ReadInterval = int (cnf.GetValue("ReadInterval"))
            WriteInterval = int (cnf.GetValue("WriteInterval"))
            Sqlite = config.GetSection("Database").GetValue("Sqlite")
            Environment = config.GetValue("Environment") 
        }

    type ReadWorker(logger : ILogger<ReadWorker>, configuration : IConfiguration ) =
        inherit BackgroundService()

        override _.ExecuteAsync(stoppingToken : CancellationToken) =
            
            let settings = getSettings configuration
            let logFound = sprintf "READING [%s]"
            let separator = "------------------------------------------------------------"
            task {
                while not stoppingToken.IsCancellationRequested do                    
                    let files = System.IO.Directory.GetFiles(settings.ReceivedFilePath)
                    if files.Length = 0 then
                        logger.LogInformation(separator)
                        logger.LogInformation("No files found.")
                    for file in files do
                        logger.LogInformation separator
                        logger.LogInformation file
                        KissUtil.getRecords settings.ReceivedFilePath None
                        |> Array.iter (fun r -> logger.LogInformation (logFound r))
                        let fileInfo = new System.IO.FileInfo(file)
                        
                        let logResults (r:Result<int, exn>) =
                            match r with
                            | Ok _ -> ()
                            | Error e -> logger.LogInformation e.Message

                        KissUtil.saveReveivedRawRecords settings.Sqlite settings.ReceivedFilePath (Some fileInfo.Name)
                        |> Array.iter logResults                        
                        
                        System.IO.File.Move (file, (sprintf "%s/processed/%s" settings.ReceivedFilePath fileInfo.Name))
                    do! Task.Delay(settings.ReadInterval, stoppingToken) 
            } :> Task

    type WriteWorker(logger : ILogger<WriteWorker>, configuration : IConfiguration ) =
        inherit BackgroundService()
        override _.ExecuteAsync(stoppingToken : CancellationToken) =
            let settings = getSettings configuration

            task {
                while not stoppingToken.IsCancellationRequested do
                    logger.LogInformation("Checking for transmit frames.")
                    let result = KissUtil.writeStoredFramesToKissUtil settings.Sqlite None settings.TransmitFilePath
                    match result with
                    | Ok (file, ids) when not (System.String.IsNullOrEmpty(file)) ->  logger.LogInformation(sprintf "WROTE kiss util file [%s]" file)
                                                                                      let! txResult = KissUtil.setTransmitted settings.Sqlite (ids |> Seq.toList) 
                                                                                      match txResult with
                                                                                      | Ok _ -> logger.LogInformation("Transmitted records were set to transmitted in the database.")
                                                                                      | Error exn -> logger.LogError(sprintf "ERROR updating transmitted records [%s]" exn.Message)
                    | Ok (_, _) -> logger.LogInformation("No new frames to transmit.")
                    | Error exn -> logger.LogError(sprintf "ERROR writing to transmit file [%s]" exn.Message)
                    do! Task.Delay(settings.WriteInterval, stoppingToken)
                            
            } :> Task
