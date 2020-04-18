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


