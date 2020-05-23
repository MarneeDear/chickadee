module PositionReportComposer

open System
open chickadee.core.TNC2MON
open chickadee.core
open Argu
open CommandArguments
open chickadee.core.PositionReport

let composePositionReport (pRpt: ParseResults<PositionReportArguments>) : PositionReport.PositionReport = 
    let latArgu = pRpt.GetResult(CommandArguments.Latitude)
    let lonArgu = pRpt.GetResult(CommandArguments.Longitude)
    let lat     = PositionReport.FormattedLatitude.create latArgu
    let lon     = PositionReport.FormattedLongitude.create lonArgu
    let symbol  = SymbolCode.fromSymbol ((pRpt.TryGetResult(CommandArguments.Symbol)) |> Option.defaultValue '-')
    let comment = pRpt.TryGetResult(CommandArguments.Comment) |> Option.defaultValue String.Empty
    { 
        Position = { Latitude = lat; Longitude = lon }
        Symbol = (if symbol.IsSome then symbol.Value else SymbolCode.House)
        TimeStamp = Some (chickadee.core.Timestamp.TimeStamp.create chickadee.core.Timestamp.TimeZone.Local)
        Comment = PositionReport.PositionReportComment.create comment
    }

let composePositionReportPacket (pRpt: ParseResults<PositionReportArguments>) (sender:CallSign) (destination:CallSign) = 
    let latArgu = pRpt.GetResult(CommandArguments.Latitude)
    let lonArgu = pRpt.GetResult(CommandArguments.Longitude)
    let lat     = PositionReport.FormattedLatitude.create latArgu
    let lon     = PositionReport.FormattedLongitude.create lonArgu
    let symbol  = SymbolCode.fromSymbol ((pRpt.TryGetResult(CommandArguments.Symbol)) |> Option.defaultValue '-')
    let comment = pRpt.TryGetResult(CommandArguments.Comment) |> Option.defaultValue String.Empty
    let information : Information =
        { 
            Position = { Latitude = lat; Longitude = lon }
            Symbol = (if symbol.IsSome then symbol.Value else SymbolCode.House)
            TimeStamp = Some (chickadee.core.Timestamp.TimeStamp.create chickadee.core.Timestamp.TimeZone.Local)
            Comment = PositionReport.PositionReportComment.create comment
        } |> PositionReport.PositionReportFormat.PositionReportWithoutTimeStampWithMessaging //todo support all types
        |> Information.PositionReport
    {
        Sender = sender
        Destination = destination
        Path = WIDEnN WIDE11
        Information = Some information
    } |> Ok

