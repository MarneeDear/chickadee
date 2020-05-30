namespace chickadee.core.DataFormats

open System
open chickadee.core.Message
open chickadee.core

module DataFormatType =
    let (|FormatType|_|) (info:string) =
        match info.Substring(0,1) with
        | "\x1C" -> Some APRSDataFormats.DataFormat.CurrentMicEData
        | "\x1D" -> Some APRSDataFormats.DataFormat.OldMicEData
        | "!"    -> Some (APRSDataFormats.DataFormat.PostionReport APRSDataFormats.PositionReportDataFormat.PositionReportWithoutTimeStampOrUltimeter)
        | "#"    -> Some APRSDataFormats.DataFormat.PeetBrosWeatherStation
        | "$"    -> Some APRSDataFormats.DataFormat.RawGPSDataOrUltimeter
        | "%"    -> Some APRSDataFormats.DataFormat.Argelo
        | "'"    -> Some APRSDataFormats.DataFormat.OldMicEButCurrentTMD700
        | ")"    -> Some APRSDataFormats.DataFormat.Item
        | "*"    -> Some APRSDataFormats.DataFormat.PeetBrosWeatherStation
        | "+"    -> Some APRSDataFormats.DataFormat.ShelterDataWithTime
        | ","    -> Some APRSDataFormats.DataFormat.InvalidOrTest
        | "/"    -> Some (APRSDataFormats.DataFormat.PostionReport APRSDataFormats.PositionReportDataFormat.PositionReportWithTimestampNoMessaging)
        | ":"    -> Some APRSDataFormats.DataFormat.Message
        | ";"    -> Some APRSDataFormats.DataFormat.Object
        | "<"    -> Some APRSDataFormats.DataFormat.StationCapabilities
        | "="    -> Some (APRSDataFormats.DataFormat.PostionReport APRSDataFormats.PositionReportDataFormat.PositionReportWithoutTimeStampWithMessaging)
        | ">"    -> Some APRSDataFormats.DataFormat.StatusReport
        | "?"    -> Some APRSDataFormats.DataFormat.Query
        | "@"    -> Some (APRSDataFormats.DataFormat.PostionReport APRSDataFormats.PositionReportDataFormat.PositionReportWithTimestampWithMessaging)
        | "T"    -> Some APRSDataFormats.DataFormat.TelemetryReport
        | "["    -> Some APRSDataFormats.DataFormat.MaidenheadGridLocatorBeacon
        | "_"    -> Some APRSDataFormats.DataFormat.WeatherReportWihtoutPosition
        | "`"    -> Some APRSDataFormats.DataFormat.CurrentMicEDataNotUsedInTMD700
        | "{"    -> Some APRSDataFormats.DataFormat.UserDefined
        | "}"    -> Some APRSDataFormats.DataFormat.ThirdPartyTraffic        
        | _      -> Some APRSDataFormats.DataFormat.Unsupported

module MessageActivePatterns =

    //Part of the message data type
    //See Appendix 1 page 100 APRS 1.01
    (*            
    Message Format
            | : | Addressee | : | Message Text  | Message ID | Message Number (xxxxx)
    bytes   | 1 |    9      | 1 |    0-67       |     {      |      1-5
    KG7SIO   :HELLO WORLD{12345
    0123456789012345678901234567890
    Message ID and Message Number are optional
    *)
    let (|Addressee|_|) (msg:string) =
        match msg.IndexOf(":") = 9 with
        | true  -> CallSign.create (msg.Substring(0,9).Trim())
        | false -> None

    let (|Message|_|) (msg:string) = 
        //let a = msg.IndexOf("ack")
        match msg.IndexOf(":"), msg.IndexOf("{"), msg.IndexOf("ack"), msg.IndexOf("rej") with        
        | i, j, a, r when i = 9 && j = -1 && a <> 10 && r <> 10 -> MessageText.create (msg.Substring(10)) //No Message ID        
        | i, j, a, r when i = 9 && j > 9 && j <= 78 && a <> 10 && r <> 10 -> MessageText.create (msg.Substring(10, j - 1 - i)) //With Message ID
        | _                                       -> None

    let (|MessageNumber|_|) (msg:string) =
        match msg.IndexOf(":"), msg.IndexOf("{") with
        | i, j when i = 9 && j > 9 && j <= i + 68 -> MessageNumber.create (msg.Substring(j+1).Trim())
        | i, j when i = 9 && j = -1 -> None //MessageNumber.create String.Empty
        | _, _                      -> None

    //KB2ICI-14:ack003
    //0123456789123456789
    let (|MessageAcknowledgementNumber|_|) (msg:string) =
        match msg.IndexOf(":") with
        | i when i = 9 -> match msg.Substring(i + 1).IndexOf("ack") with
                          | 0 -> MessageNumber.create (msg.Substring(14))
                          | _ -> None
        | _ -> None

    //KB2ICI-14:rej003
    let (|MessageRejectionNumber|_|) (msg:string) =
        match msg.IndexOf(":") with
        | i when i = 9 -> match msg.Substring(i + 1).IndexOf("rej") with
                          | 0 -> MessageNumber.create (msg.Substring(14))
                          | _ -> None
        | _ -> None

//Example: =3603.55N/112006.56W-
//Example: !4903.50N/07201.75W-Test 001234
//Example: /092345z4903.50N/07201.75W>Test1234
//Example: @092345/4903.50N/07201.75W>Test1234
         //0123456789012345678901234567890123456789
module PositionReportActivePatterns =

    let hasTimeStamp (info:string) =
        match info.Substring(0,1) with
        | i when i = "=" || i = "!" -> false
        | i when i = "/" || i = "@" -> true
        | _ -> false

    let (|TimeStamp|_|) (info:string) =        
        match hasTimeStamp info with
        | false -> None
        | true -> let ts = info.Substring(1, 7)
                  match ts.EndsWith("z") || ts.EndsWith("/") with
                  | true -> Some (Timestamp.TimeStamp.rxed ts)
                  | false -> None

    //Only supports Lat/Long Position Report Format — without Timestamp
    //See APRS 1.01 spec, section 8 POSITION AND DF REPORT DATA FORMATS
    //TODO make the data type identifies types
    //TODO According to APRS spec the Longitude is 8 chars fixed-length. Can just use the length to parse?    
    let (|Latitude|_|) (info:string) = 
        let lat = 
            match hasTimeStamp info with
            | true ->  info.Substring(8, 8)
            | false -> info.Substring(1, 8)
        //let lat = info.Substring(1, 8)
        match lat.EndsWith("N"), lat.EndsWith("S") with
        | true, false   -> Some lat
        | false, true   -> Some lat
        | _             -> None

    //Only supports Lat/Long Position Report Format — without Timestamp
    //See APRS 1.01 spec, section 8 POSITION AND DF REPORT DATA FORMATS
    //TODO According to APRS spec the Longitude is 9 chars fixed-length. Can just use the length to parse.
    //Example: =3603.55N/112006.56W-
    let (|Longitude|_|) (info:string) =
        //let parseLongitude (posRpt:string) =
        let lon = 
            match hasTimeStamp info with
            | false -> info.Substring(10, 9) 
            | true -> info.Substring(17, 9) 
        match lon.EndsWith("W"), lon.EndsWith("E") with 
        | true, false   -> Some lon
        | false, true   -> Some lon
        | _             -> None
            
        //match info.Substring(9,1) with
        //| "/"   -> parseLongitude info
        //| _     -> None

    //TODO can do this by string position because the lat/lon is a fixed length
    //Should be char 20
    //Example: =3603.55N/112006.56W-
    let (|Symbol|_|) (info:string) =
        //TODO check that the previous char was a W or E meaning that it was probably and APRS lat/lon
        //let sym =
        match hasTimeStamp info with
        | true -> SymbolCode.fromSymbol (info.Substring(26,1).ToCharArray().[0])
        | false -> SymbolCode.fromSymbol (info.Substring(19,1).ToCharArray().[0])
        //match sym with
        //| "W" -> SymbolCode.fromSymbol (sym.ToCharArray().[0]) 
        //| "E" -> SymbolCode.fromSymbol (sym.ToCharArray().[0])
        //| _ -> None

    let (|Comment|_|) (symbol:char) (info:string) = //TODO by index?
        let comment = info.Substring(info.IndexOf(symbol) + 1).Trim()
        match String.IsNullOrEmpty(comment) with
        | true -> None
        | false -> Some comment

