namespace PositionReports

module Model =
    type PositionReport =
        {
            ReportType : string
            Latitude : string
            Longitude : string
            SymbolImage : string
            TimeStamp : string
            Comment : string
            
        }
    type PositionReportReceived =
        {
            DateCreated : string
            RawPacket : string
            RawPosition : string
            DateReceived : string
            PositionReport : PositionReport
            Error : string
        }

    type PositionReportTransmitted = 
        {
            DateCreated : string
            RawPacket : string
            RawPosition : string
            PositionReport : PositionReport
            Error : string
            Transmitted : bool
        }