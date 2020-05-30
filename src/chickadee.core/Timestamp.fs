namespace chickadee.core

module Timestamp =
    open System

    (*
    Time Formats APRS timestamps are expressed in three different ways:
    • Day/Hours/Minutes format
    • Hours/Minutes/Seconds format
    • Month/Day/Hours/Minutes format

    In all three formats, the 24-hour clock is used.

    Day/Hours/Minutes (DHM) 
    
    format is a fixed 7-character field, consisting of

    * a 6-digit day/time group 
    * followed by a single time indicator character (z or /). 
    
    The day/time group consists of 
    
    * a two-digit day-of-the-month (01–31) and
    * a four-digit time in hours and minutes.

    Times can be expressed in zulu (UTC/GMT) or local time. 
    
    For example:
    092345z is 2345 hours zulu time on the 9th day of the month.
    092345/ is 2345 hours local time on the 9th day of the month.

    It is recommended that future APRS implementations only transmit zulu
    format on the air.

    Note: The time in Status Reports may only be in zulu format.
    Hours/Minutes/Seconds (HMS) format is a fixed 7-character field,
    consisting of a 6-digit time in hours, minutes and seconds, followed by the h
    time-indicator character. 

    For example:
    234517h is 23 hours 45 minutes and 17 seconds zulu.

    Note: This format may not be used in Status Reports.
    Month/Day/Hours/Minutes (MDHM) format is a fixed 8-character field,
    consisting of the month (01–12) and day-of-the-month (01–31), followed by
    the time in hours and minutes zulu. For example:
    10092345 is 23 hours 45 minutes zulu on October 9th.

    This format is only used in reports from stand-alone “positionless” weather
    stations (i.e. reports that do not contain station position information).
    *)

    type TimeZone =
        | Zulu
        | Local
        override this.ToString() =
            match this with
            | Zulu  -> "z"
            | Local -> "/"

    //TODO write tests
    type TimeStamp = private TimeStamp of string
    module TimeStamp =
        let rxed (ts:string) =
            TimeStamp ts
        let create (z:TimeZone) =
            TimeStamp (sprintf "%02i%02i%02i%s" DateTime.Now.Day DateTime.Now.Hour DateTime.Now.Minute (z.ToString()))
        let value (TimeStamp t) = t