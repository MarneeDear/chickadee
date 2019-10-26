namespace faprs.core

open System
open Message
open PositionReport

//TODO module APRSDataExtensions
//TODO how to model SSIDs ?  APRS SSID Recommendations
//TODO include SSIDs
//http://www.aprs.org/aprs11/SSIDs.txt

[<AutoOpen>]
module APRSData = 

    type Information =
        | Message                           of MessageType
        | PositionReportWithoutTimeStamp    of PositionReportWithoutTimeStamp
        | ParticipantStatusReport           of Participant.ParticipantStatusReport
        | Unsupported                       of string
        override this.ToString() =
            match this with 
            | Message m                         -> m.ToString()
            | PositionReportWithoutTimeStamp r  -> r.ToString()
            | ParticipantStatusReport r         -> r.ToString()
            | Unsupported u                     -> u //This is where anything that cant be parsed will end up

