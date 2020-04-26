namespace PositionReports

module Model =
    type PositionReportReceived =
        {
            ReportType : string
            Latitude : string
            Longitude : string
            Symbol : string
            TimeStamp : string
            Comment : string
            DateReceived : string
        }

    type PositionReportTransmitted = 
        {
            ReportType : string
            Latitude : string
            Longitude : string
            Symbol : string
            TimeStamp : string
            Comment : string
            DateReceived : string
        }