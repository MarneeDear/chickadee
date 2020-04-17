namespace chickadee.core

open System
open PositionReport
(*

TNC2 MONitor packet format.

This is the standard packet format used by TNC (Terminal Node Controller) v2 devices.
This format is specified in the APRS version 1.0.1 under Network Tunneling and Thrid-Party Digipeating 
The packet consists of a source path header and a message, usually a Position Report

FORMAT: SENDER>DESTINATION,PATH:MESSAGE
FORMAT: CALLSIGN>CALLSIGN,PATH:POSITION REPORT
FORMAT: CALLSIGN>TOCALL,PATH:MESSAGE

TOCALL:

In APRS the destination field does not have to be a specific call sign. Instead that field is overloaded and can be used to pass on other
encoded buts of information. One common usage is to identify the sending application and version number. You can see a list of TAPR approved to-calls here:
http://aprs.org/aprs11/tocalls.txt

PATH:
http://wa8lmf.net/DigiPaths/
http://wa8lmf.net/DigiPaths/NNNN-Digi-Demo.htm

EXAMPLE: KG7SIO-7>APDW15,WIDE1-1:=3000.1N/1000.1W-

From KG7SIO-1
Using DireWolf version 1.5 (We are using Dire Wolf to transmit our messages over the radio. The packages are crafted by this code can be used to produce a kissutil file)
Over WIDE1-1
With a position report

*)
module TNC2MON = 
    open Message

    type RawInformation =
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

    let getRawPaketType id =
        match id with
        | "\u1c" -> CurrentMicEData
        | "\u1d" -> OldMicEData
        | "!" -> PositionReportWithoutTimeStampOrUltimeter
        | "#" -> PeetBrosWeatherStation
        | "$" -> RawGPSDataOrUltimeter
        | "%" -> Argelo
        | "'" -> OldMicEButCurrentTMD700
        | ")" -> Item
        | "*" -> PeetBrosWeatherStation
        | "+" -> ShelterDataWithTime
        | "," -> InvalidOrTest
        | "/" -> PositionReportWithTimestampNoMessaging
        | ":" -> Message
        | ";" -> Object
        | "<" -> StationCapabilities
        | "=" -> PositionReportWithoutTimeStampWithMessaging
        | ">" -> StatusReport
        | "?" -> Query
        | "@" -> PositionReportWithTimestampWithMessaging
        | "T" -> TelemetryReport
        | "[" -> MaidenheadGridLocatorBeacon
        | "{" -> UserDefined
        | "}" -> ThirdPartyTraffic        
        | _   -> Unsupported

    type Information =
        | Message                           of MessageFormat
        | PositionReport                    of PositionReportType //PositionReportWithoutTimeStamp
        | ParticipantStatusReport           of Participant.ParticipantStatusReport
        | Unsupported                       of string
        override this.ToString() =
            match this with 
            | Message m                 -> match m with
                                           | MessageFormat.Message r -> r.ToString()
                                           | _ -> "NOT IMPLEMENTED"
            | PositionReport r          -> match r with
                                           //| _ -> r.ToString()
                                           | PositionReportType.PositionReportWithoutTimeStampOrUltimeter p     -> p.ToString()
                                           | PositionReportType.PositionReportWithTimestampNoMessaging p        -> p.ToString()
                                           | PositionReportType.PositionReportWithoutTimeStampWithMessaging p   -> p.ToString()
                                           | PositionReportType.PositionReportWithTimestampWithMessaging p      -> p.ToString()
            | ParticipantStatusReport r -> r.ToString()
            | Unsupported u             -> u //This is where anything that cant be parsed will end up

    type Packet = 
        {
            Sender      : CallSign //9 bytes
            Destination : CallSign //9 bytes
            Path        : Path //81 bytes, TODO this can be a list 
            Information : Information option
        }
        override this.ToString() =
            let info =
                match this.Information with
                | Some i  -> i.ToString()
                | None    -> String.Empty
            sprintf "%s>%s,%s:%s" (CallSign.value this.Sender) (CallSign.value this.Destination) (this.Path.ToString()) info


