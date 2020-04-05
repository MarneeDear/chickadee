namespace chickadee.service

module Settings =

    type WorkerOptions =
        {
            TransmitFilePath : string
            TransmitFileNameSuffix : string
            ReceivedFilePath : string
            ReadInterval : int
            WriteInterval : int
            Sqlite : string
            Environment : string
        }
    type DatabaseSettings =
        {
            Sqlite : string
        }

