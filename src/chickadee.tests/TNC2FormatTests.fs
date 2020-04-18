module TNC2FormatTests

open Expecto
open chickadee.core.TNC2MON
open chickadee.core.Common
open chickadee.core.PositionReport
open chickadee.core.DataFormats.DataFormatType
open chickadee.core.APRSDataFormats

[<Literal>]
let SENDER = "kg7sio"
[<Literal>]
let DESTINATION = "apdw15" //DireWolf v1.5 ToCall
[<Literal>]
let PATH = "WIDE1-1"
let LATITUDE = 36.0591117 //DD decimal degrees
let LONGITUDE = -112.1093343 //DD decimal degrees
let LONGITUDE_HEMISPHERE = East // 'E'
let LATITUDE_HEMISPHERE =  North // 'N'
let POSITION_REPORT_HOUSE = sprintf "=%.2f%c/%.2f%c-" LATITUDE (LATITUDE_HEMISPHERE.ToHemisphereChar()) LONGITUDE (LONGITUDE_HEMISPHERE.ToHemisphereChar())
let FINAL_POSITION_REPORT = "=3603.33N/11206.34W-"
let TNC2_FINAL = (sprintf "%s>%s,%s:%s" (SENDER.ToUpper()) (DESTINATION.ToUpper()) PATH FINAL_POSITION_REPORT)


let PACKET_POSITION_REPORT_HOUSE =
    { 
        Position = { 
            Latitude = FormattedLatitude.create LATITUDE 
            Longitude = FormattedLongitude.create LONGITUDE 
        } 
        Symbol = House
        Comment = None //(PositionReportComment.create String.Empty).Value
        TimeStamp = None
    }

// TODO: introduce property based testsing?
// TODO: more tests for all position report types when possible
[<Tests>]
let TNC2MONFormatTests =
    testList "TNC2MON Format Tests" [
        testCase "Can build a packet with Position Report with latitude and longitude and upper call sign" <| fun _ ->
            let packet = 
                {
                    Sender      = (CallSign.create (SENDER.ToUpper())).Value
                    Destination = (CallSign.create (DESTINATION.ToUpper())).Value
                    Path        = WIDEnN WIDE11 //"WIDE1-1"
                    Information = Some (chickadee.core.PositionReport.PositionReportWithTimestampNoMessaging PACKET_POSITION_REPORT_HOUSE 
                                  |> Information.PositionReport)
                }.ToString()
            // Console.WriteLine packet
            Expect.equal packet TNC2_FINAL (sprintf "TNC2MON formats didnt match")
        testCase "Can build a packet with Position Report with latitude and longitude and lower callsign goes to upper" <| fun _ ->
            let packet = 
                {
                    Sender      = (CallSign.create SENDER).Value
                    Destination = (CallSign.create DESTINATION).Value
                    Path        = WIDEnN WIDE11 //"WIDE1-1"
                    Information = Some (chickadee.core.PositionReport.PositionReportWithoutTimeStampWithMessaging PACKET_POSITION_REPORT_HOUSE 
                                  |> Information.PositionReport)
                }.ToString()
            // Console.WriteLine packet
            Expect.equal packet TNC2_FINAL (sprintf "TNC2 formats didnt match")
    ]

[<Tests>]
let RawPacketTypeTests =
    testList "Raw Packet Type Tests" [
        testCase "Current MicE Data" <| fun _ ->
            //let code = sprintf "%cblahblah" (char "\u1c")
            //System.Text.UnicodeEncoding.UTF8.
            Expect.equal ((|FormatType|_|) "\x1C blaaaa").Value CurrentMicEData "CurrentMicEData"
        testCase "Old MicE Data" <| fun _ ->
            Expect.equal ((|FormatType|_|) "\x1D blaaa").Value OldMicEData "OldMicEData"
        testCase "Position Report Without TimeStamp Or Ultimeter" <| fun _ ->
            Expect.equal ((|FormatType|_|) "! blahblah").Value PositionReportWithoutTimeStampOrUltimeter "PositionReportWithoutTimeStampOrUltimeter"
        testCase "Peet Bros Weather Station #" <| fun _ ->
            Expect.equal ((|FormatType|_|) "# blahblah").Value PeetBrosWeatherStation "PeetBrosWeatherStation"
            //Expect.equal (getRawPaketType "#") RawInformation.PeetBrosWeatherStation "Peet Bros Weather Station #"
        testCase "Raw GPS Data Or Ultimeter" <| fun _ ->
            Expect.equal ((|FormatType|_|) "$ blahblah").Value RawGPSDataOrUltimeter "RawGPSDataOrUltimeter"
        testCase "Argelo" <| fun _ ->
            Expect.equal ((|FormatType|_|) "% blahblah").Value Argelo "Argelo"
            //Expect.equal (getRawPaketType "%") RawInformation.Argelo "Argelo"
        testCase "Old MicE But Current TMD700" <| fun _ ->
            Expect.equal ((|FormatType|_|) "' blahblah").Value OldMicEButCurrentTMD700 "OldMicEButCurrentTMD700"
            //Expect.equal (getRawPaketType "'") RawInformation.OldMicEButCurrentTMD700 "Old MicE But Current TMD700"
        testCase "Item" <| fun _ ->
            Expect.equal ((|FormatType|_|) ") blahblah").Value Item "Item"
            //Expect.equal (getRawPaketType ")") RawInformation.Item "Item"
        testCase "Peet Bros Weather Station *" <| fun _ ->
            Expect.equal ((|FormatType|_|) "* blahblah").Value PeetBrosWeatherStation "PeetBrosWeatherStation"
            //Expect.equal (getRawPaketType "*") RawInformation.PeetBrosWeatherStation "Peet Bros Weather Station"
        testCase "Shelter Data With Time" <| fun _ ->
            Expect.equal ((|FormatType|_|) "+ blahblah").Value ShelterDataWithTime "ShelterDataWithTime"
            //Expect.equal (getRawPaketType "+") RawInformation.ShelterDataWithTime "Shelter Data With Time"
        testCase "Invalid Or Test" <| fun _ ->
            Expect.equal ((|FormatType|_|) ", blahblah").Value InvalidOrTest "InvalidOrTest"
            //Expect.equal (getRawPaketType ",") RawInformation.InvalidOrTest "Invalid Or Test"
        testCase "Position Report With Timestamp No Messaging" <| fun _ ->
            Expect.equal ((|FormatType|_|) "/ blahblah").Value PositionReportWithTimestampNoMessaging "PositionReportWithTimestampNoMessaging"
            //Expect.equal (getRawPaketType "/") RawInformation.PositionReportWithTimestampNoMessaging ""
        testCase "Message" <| fun _ ->
            Expect.equal ((|FormatType|_|) ": blahblah").Value Message "Message"
            //Expect.equal (getRawPaketType ":") RawInformation.Message "Message"
        testCase "Object" <| fun _ ->
            Expect.equal ((|FormatType|_|) "; blahblah").Value Object "Object"
            //Expect.equal (getRawPaketType ";") RawInformation.Object "Object"
        testCase "Station Capabilities" <| fun _ ->
            Expect.equal ((|FormatType|_|) "< blahblah").Value StationCapabilities "StationCapabilities"
            //Expect.equal (getRawPaketType "<") RawInformation.StationCapabilities "Station Capabilities"
        testCase "Position Report Without TimeStamp With Messaging" <| fun _ ->
            Expect.equal ((|FormatType|_|) "= blahblah").Value PositionReportWithoutTimeStampWithMessaging "PositionReportWithoutTimeStampWithMessaging"
            //Expect.equal (getRawPaketType "=") RawInformation.PositionReportWithoutTimeStampWithMessaging ""
        testCase "Status Report" <| fun _ ->
            Expect.equal ((|FormatType|_|) "> blahblah").Value StatusReport "StatusReport"
            //Expect.equal (getRawPaketType ">") RawInformation.StatusReport "Status Report"
        testCase "Query" <| fun _ ->
            Expect.equal ((|FormatType|_|) "? blahblah").Value Query "Query"
            //Expect.equal (getRawPaketType "?") RawInformation.Query "Query"
        testCase "Position Report With Timestamp With Messaging" <| fun _ ->
            Expect.equal ((|FormatType|_|) "@ blahblah").Value PositionReportWithTimestampWithMessaging "PositionReportWithTimestampWithMessaging"
            //Expect.equal (getRawPaketType "@") RawInformation.PositionReportWithTimestampWithMessaging ""
        testCase "T is Telemetry Report" <| fun _ ->
            Expect.equal ((|FormatType|_|) "T blahblah").Value TelemetryReport "TelemetryReport"
            //Expect.equal (getRawPaketType "T") RawInformation.TelemetryReport "Should be Telemetry"
        testCase "[ Maidenhead Grid Locator Beacon" <| fun _ ->
            Expect.equal ((|FormatType|_|) "[ blahblah").Value MaidenheadGridLocatorBeacon "MaidenheadGridLocatorBeacon"
            //Expect.equal (getRawPaketType "[") RawInformation.MaidenheadGridLocatorBeacon "Should be Maidenhead Grid Locator Beacon"
        testCase "" <| fun _ ->
            Expect.equal ((|FormatType|_|) "{ blahblah").Value UserDefined "UserDefined"
            //Expect.equal (getRawPaketType "{") RawInformation.UserDefined "Should be UserDefined"
        testCase "} is Third Party" <| fun _ ->
            Expect.equal ((|FormatType|_|) "} blahblah").Value ThirdPartyTraffic "ThirdPartyTraffic"
            //Expect.equal (getRawPaketType "}") RawInformation.ThirdPartyTraffic "Should be Third Party"
        testCase "Unsupported character" <| fun _ ->
            Expect.equal ((|FormatType|_|) "N blahblah").Value Unsupported "TelemetryReport"
            //Expect.equal (getRawPaketType "N") RawInformation.Unsupported "Should be unsupported"
    ]
