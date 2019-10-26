namespace chickadee.core

module PositionReport =
    open System

    type PositionReportComment = private PositionReportComment of string
    module PositionReportComment =
        let create (s:string) =
            match (s.Trim()) with
            | s when s.Length < 44  -> Some (PositionReportComment s)
            | _                     -> None 

        let value (PositionReportComment c) = c //Was trimmed during create

    type LatitudeHemisphere =
        | North     
        | South     
        member this.ToHemisphereChar() =
            match this with
            | North   -> 'N'
            | South   -> 'S'
        static member fromHemisphere h =
            match h with
            | 'N'   -> Some LatitudeHemisphere.North
            | 'S'   -> Some LatitudeHemisphere.South
            | _     -> None //"Latitude must be in northern (N) or southern (S) hemisphere."

    type LongitudeHemisphere = 
        | East      
        | West      
        member this.ToHemisphereChar() =
            match this with
            | East    -> 'E'
            | West    -> 'W'
        static member fromHemisphere h =
            match h with
            | 'E'   -> Some LongitudeHemisphere.East
            | 'W'   -> Some LongitudeHemisphere.West
            | _     -> None 

    let hemisphereToString degrees hemisphereChar =
        sprintf "%.2f%c" degrees hemisphereChar

    let calcDegMinSec (d:float) =
        let dd = Math.Abs(d)
        let deg = Math.Floor(dd)
        let min = (dd - deg) * 60.0
        let sec = (dd - deg - (Math.Floor(min) / 60.0)) * 3600.0
        let rsec = Math.Round((decimal sec), 0)
        int deg, int min, int rsec

    //TODO constrain the size of the Degrees field
    (* 
    APRS101.pdf Chapter 6: Time and Position Formats
    Latitude is expressed as a fixed 8-character field, in degrees and decimal
    minutes (to two decimal places), followed by the letter N for north or S for
    south.

    Latitude degrees are in the range 00 to 90. Latitude minutes are expressed as
    whole minutes and hundredths of a minute, separated by a decimal point.
    
    For example:
    4903.50N is 49 degrees 3 minutes 30 seconds north.
    In generic format examples, the latitude is shown as the 8-character string
    ddmm.hhN (i.e. degrees, minutes and hundredths of a minute north).

    *)
    type FormattedLatitude = private FormattedLatitude of string
    module FormattedLatitude =
        let create (d:float) =
            let deg, min, sec = calcDegMinSec d
            FormattedLatitude (sprintf "%02i%02i.%02i%c" deg min sec (if d > 0.0 then (North.ToHemisphereChar()) else (South.ToHemisphereChar())))
        let check (d:string) =
            FormattedLatitude d //TODO verify in expected format -- regular expressions?
        let value (FormattedLatitude d) = d

    (* 
    APRS101.pdf Chapter 6: Time and Position Formats
    Longitude is expressed as a fixed 9-character field, in degrees and decimal
    minutes (to two decimal places), followed by the letter E for east or W for
    west.

    Longitude degrees are in the range 000 to 180. Longitude minutes are
    expressed as whole minutes and hundredths of a minute, separated by a
    decimal point.
    For example:
    07201.75W is 72 degrees 1 minute 45 seconds west.
    In generic format examples, the longitude is shown as the 9-character string
    dddmm.hhW (i.e. degrees, minutes and hundredths of a minute west).
    *)
    type FormattedLongitude = private FormattedLongitude of string
    module FormattedLongitude =
        let create (d:float) =
            let deg, min, sec = calcDegMinSec d
            FormattedLongitude (sprintf "%03i%02i.%02i%c" deg min sec (if d > 0.0 then (East.ToHemisphereChar()) else (West.ToHemisphereChar())))
        let check (d:string) =
            FormattedLongitude d //TODO verify in expected format -- regular expressions?
        let value (FormattedLongitude d) = d

    type Position =
        {
            Latitude    : FormattedLatitude 
            Longitude   : FormattedLongitude
        }

    //TODO support more position report types -- data extensions
    (*
    APRS101.pdf Chapter 6: Time and Position Formats
    Must end in a Symbol Code
    Position coordinates are a combination of latitude and longitude, separated
    by a display Symbol Table Identifier, and followed by a Symbol Code. 
    *)
    type PositionReportWithoutTimeStamp =
        {
            Position    : Position
            Symbol      : SymbolCode
            Comment     : PositionReportComment option
        }
        override this.ToString() =
            let comment =
                match this.Comment with
                | Some c    -> PositionReportComment.value c
                | None      -> String.Empty
            sprintf "=%s/%s%c%s" (FormattedLatitude.value this.Position.Latitude) (FormattedLongitude.value this.Position.Longitude) (this.Symbol.ToChar()) comment
    

    type PositionReportType =
        | PositionReportWithoutTimeStamp of PositionReportWithoutTimeStamp
