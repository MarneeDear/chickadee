namespace chickadee.core

module APRSDataFormats =

    type DataFormat =
        | CurrentMicEData
        | OldMicEData
        | PositionReportWithoutTimeStampOrUltimeter
        | PeetBrosWeatherStation
        | RawGPSDataOrUltimeter
        | Argelo
        | OldMicEButCurrentTMD700
        | Item
        | ShelterDataWithTime
        | InvalidOrTest
        | PositionReportWithTimestampNoMessaging
        | Message
        | Object
        | StationCapabilities
        | PositionReportWithoutTimeStampWithMessaging
        | StatusReport
        | Query
        | PositionReportWithTimestampWithMessaging
        | TelemetryReport
        | MaidenheadGridLocatorBeacon
        | WeatherReportWihtoutPosition
        | CurrentMicEDataNotUsedInTMD700
        | UserDefined
        | ThirdPartyTraffic
        | Unsupported
        override this.ToString() =
            match this with
            | CurrentMicEData                               -> "Current Mic-E Data"
            | OldMicEData                                   -> "Old Mic-E Data"
            | PositionReportWithoutTimeStampOrUltimeter     -> "Position Report without timestamp or Ultimeter station"
            | PeetBrosWeatherStation                        -> "Peet Bros Weather Stations"
            | RawGPSDataOrUltimeter                         -> "Raw GPS Data or Ultimeter station"
            | Argelo                                        -> "Argelo DFJr/MicroFinder"
            | OldMicEButCurrentTMD700                       -> "Old Mic-E Data (but Current data for TM-D700)"
            | Item                                          -> "Item"
            | ShelterDataWithTime                           -> "[Reserved — Shelter data with time]"
            | InvalidOrTest                                 -> "Invalid or Test" 
            | PositionReportWithTimestampNoMessaging        -> "Position with timestamp (no APRS messaging)"
            | Message                                       -> "Message"
            | Object                                        -> "Object"
            | StationCapabilities                           -> "Station Capabilities"
            | PositionReportWithoutTimeStampWithMessaging   -> "Position without timestamp (with APRS messaging)"
            | StatusReport                                  -> "Status Report"
            | Query                                         -> "Query"
            | PositionReportWithTimestampWithMessaging      -> "Position with timestamp (with APRS messaging)"
            | TelemetryReport                               -> "Telemetry Report"
            | MaidenheadGridLocatorBeacon                   -> "Maidenhead grid locator beacon (obsolete)"
            | WeatherReportWihtoutPosition                  -> "Weather Report (without position)"
            | CurrentMicEDataNotUsedInTMD700                -> "Current Mic-E Data (not used in TM-D700)"
            | UserDefined                                   -> "User Defined"
            | ThirdPartyTraffic                             -> "Third-party traffic"
            | Unsupported                                   -> "Unsupported"

    //let getRawPaketType id =
    //    match id with
    //    | "\u1c" -> CurrentMicEData
    //    | "\u1d" -> OldMicEData
    //    | "!" -> PositionReportWithoutTimeStampOrUltimeter
    //    | "#" -> PeetBrosWeatherStation
    //    | "$" -> RawGPSDataOrUltimeter
    //    | "%" -> Argelo
    //    | "'" -> OldMicEButCurrentTMD700
    //    | ")" -> Item
    //    | "*" -> PeetBrosWeatherStation
    //    | "+" -> ShelterDataWithTime
    //    | "," -> InvalidOrTest
    //    | "/" -> PositionReportWithTimestampNoMessaging
    //    | ":" -> Message
    //    | ";" -> Object
    //    | "<" -> StationCapabilities
    //    | "=" -> PositionReportWithoutTimeStampWithMessaging
    //    | ">" -> StatusReport
    //    | "?" -> Query
    //    | "@" -> PositionReportWithTimestampWithMessaging
    //    | "T" -> TelemetryReport
    //    | "[" -> MaidenheadGridLocatorBeacon
    //    | "{" -> UserDefined
    //    | "}" -> ThirdPartyTraffic        
    //    | _   -> Unsupported


