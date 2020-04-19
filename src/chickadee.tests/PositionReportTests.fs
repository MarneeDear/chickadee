module PositionReportTests

open Expecto
open chickadee.core.Common
open chickadee.core.PositionReport
open chickadee.core.Timestamp

[<Literal>]
let LATITUDE = 36.0591117 //DD decimal degrees
let LONGITUDE = -112.1093343 //DD decimal degrees
let LONGITUDE_HEMISPHERE = East // 'E'
let LATITUDE_HEMISPHERE =  North // 'N'
let POSITION_REPORT_HOUSE = sprintf "=%.2f%c/%.2f%c-" LATITUDE (LATITUDE_HEMISPHERE.ToHemisphereChar()) LONGITUDE (LONGITUDE_HEMISPHERE.ToHemisphereChar())
[<Literal>]
let FINAL_POSITION_REPORT = "=3603.33N/11206.34W-"



let POSITION_REPORT_NO_TIMESTAMP_WITH_COMMENT =
    { 
        Position = { 
            Latitude = FormattedLatitude.create LATITUDE 
            Longitude = FormattedLongitude.create LONGITUDE 
        } 
        Symbol = House
        Comment = (PositionReportComment.create "HELLO WORLD")  
        TimeStamp = None
    }

let POSITION_REPORT_WITH_TIMESTAMP_NO_COMMENT (tz:TimeZone) =
    { 
        Position = { 
            Latitude = FormattedLatitude.create LATITUDE 
            Longitude = FormattedLongitude.create LONGITUDE 
        } 
        Symbol = House
        Comment = None 
        TimeStamp = Some (TimeStamp.create tz)
    }

let POSITION_REPORT_WITH_TIMESTAMP_WITH_COMMENT (tz:TimeZone) =
    { 
        Position = { 
            Latitude = FormattedLatitude.create LATITUDE 
            Longitude = FormattedLongitude.create LONGITUDE 
        } 
        Symbol = House
        Comment = (PositionReportComment.create "HELLO WORLD") 
        TimeStamp = Some (TimeStamp.create tz)
    }

let POSITION_REPORT_NO_TIMESTAMP_NO_COMMENT =
    { 
        Position = { 
            Latitude = FormattedLatitude.create LATITUDE 
            Longitude = FormattedLongitude.create LONGITUDE 
        } 
        Symbol = House
        Comment = None
        TimeStamp = None
    }

let CUR_TIMESTAMP_ZULU = TimeStamp.value (TimeStamp.create TimeZone.Zulu)
let CUR_TIMESTAMP_LOCAL = TimeStamp.value (TimeStamp.create TimeZone.Local)

[<Tests>]
let PositionReportTests =
    testList "Position Report Format Tests" [
        testCase "Can create position report with timestamp with messaging with Zulu timezone" <| fun _ ->
            let posRpt = PositionReportFormat.PositionReportWithTimestampWithMessaging (POSITION_REPORT_WITH_TIMESTAMP_WITH_COMMENT TimeZone.Zulu)
            let posRptTst = sprintf "@%s3603.33N/11206.34W-HELLO WORLD" CUR_TIMESTAMP_ZULU
            Expect.equal (posRpt.ToString()) posRptTst "Position report not in expected format"
        testCase "Can create position report with timestamp with messaging with Local timezone" <| fun _ ->
            let posRpt = PositionReportFormat.PositionReportWithTimestampWithMessaging (POSITION_REPORT_WITH_TIMESTAMP_WITH_COMMENT TimeZone.Local)
            let posRptTst = sprintf "@%s3603.33N/11206.34W-HELLO WORLD" CUR_TIMESTAMP_LOCAL
            Expect.equal (posRpt.ToString()) posRptTst "Position report not in expected format"
        testCase "Can create position report with timestamp no messaging with Zulu timezone" <| fun _ ->
            let posRpt = PositionReportFormat.PositionReportWithTimestampNoMessaging (POSITION_REPORT_WITH_TIMESTAMP_NO_COMMENT TimeZone.Zulu)
            let posRptTst = sprintf "/%s3603.33N/11206.34W-" CUR_TIMESTAMP_ZULU
            Expect.equal (posRpt.ToString()) posRptTst "Position report not in expected format"
        testCase "Can create position report with timestamp no messaging with Local timezone" <| fun _ ->
            let posRpt = PositionReportFormat.PositionReportWithTimestampNoMessaging (POSITION_REPORT_WITH_TIMESTAMP_NO_COMMENT TimeZone.Local)
            let posRptTst = sprintf "/%s3603.33N/11206.34W-" CUR_TIMESTAMP_LOCAL
            Expect.equal (posRpt.ToString()) posRptTst "Position report not in expected format"
        testCase "Can create position report no timestamp no messaging" <| fun _ ->
            let posRpt = PositionReportFormat.PositionReportWithoutTimeStampOrUltimeter POSITION_REPORT_NO_TIMESTAMP_NO_COMMENT
            let posRptTst = "!3603.33N/11206.34W-" 
            Expect.equal (posRpt.ToString()) posRptTst "Position report not in expected format"
        testCase "Can create position report no timestamp with messaging" <| fun _ ->
            let posRpt = PositionReportFormat.PositionReportWithTimestampWithMessaging POSITION_REPORT_NO_TIMESTAMP_WITH_COMMENT
            let posRptTst = "=3603.33N/11206.34W-HELLO WORLD" 
            Expect.equal (posRpt.ToString()) posRptTst "Position report not in expected format"

    ]