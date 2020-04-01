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

    let settings (config:IConfiguration) : Settings.DireWolfSettings = //configuration.GetSection("DireWolf").Get<Settings.DireWolfSettings>()
        let cnf = config.GetSection("DireWolf")
        {
            TransmitFilePath = cnf.GetValue("ReadInterval")
            TransmitFileNameSuffix = cnf.GetValue("TransmitFileNameSuffix")
            ReceivedFilePath = cnf.GetValue("ReceivedFilePath")
            ReadInterval = int (cnf.GetValue("ReadInterval"))
            WriteInterval = int (cnf.GetValue("WriteInterval"))
        }

    type ReadWorker(logger : ILogger<ReadWorker>, configuration : IConfiguration ) =
        inherit BackgroundService()

        override _.ExecuteAsync(stoppingToken : CancellationToken) =
            let st = settings configuration
            let logFound = sprintf "READING [%s]"
            let separator = "------------------------------------------------------------"
            task {
                while not stoppingToken.IsCancellationRequested do                    
                    let files = System.IO.Directory.GetFiles(st.ReceivedFilePath)
                    if files.Length = 0 then
                        logger.LogInformation(separator)
                        logger.LogInformation("No files found.")
                    for file in files do
                        logger.LogInformation(separator)
                        for result in (KissUtil.processKissUtilFramesInFile file) do
                            logger.LogInformation(separator)
                            let frame =
                                match result with
                                | Ok fr -> match fr with
                                           | TNC2MON.Information.Message m                 -> logger.LogInformation(logFound (m.GetType().ToString()))
                                                                                  //TODO do something with this -- add to the database
                                           | TNC2MON.Information.PositionReport m          -> logger.LogInformation(logFound (m.GetType().ToString()))
                                           | TNC2MON.Information.ParticipantStatusReport m -> logger.LogInformation(logFound (m.GetType().ToString()))
                                           | TNC2MON.Information.Unsupported m             -> logger.LogInformation(logFound (m.GetType().ToString()))
                                           logger.LogInformation(fr.ToString())
                                | Error msg -> failwith msg
                            logger.LogInformation(separator)
                    do! Task.Delay(st.ReadInterval, stoppingToken) 
                    //do! Task.Delay(10000, stoppingToken)
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
