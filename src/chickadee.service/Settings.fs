namespace chickadee.service

module Settings =

    type DireWolfSettings =
        {
            TransmitFilePath : string
            TransmitFileNameSuffix : string
            ReceivedFilePath : string
            ReadInterval : int
            WriteInterval : int
            Sqlite : string
        }