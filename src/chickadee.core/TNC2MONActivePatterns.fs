namespace chickadee.core

open System

module TNC2MonActivePatterns = 

//TODO map into their respective types, especially the non-string values
    //Remove the channel from the frame
    let (|Frame|_|) (record:string) =
        match record with
        | r when String.IsNullOrWhiteSpace(r) -> None
        | r when r.IndexOf(" ") < 1 -> None //maybe return r because there was no channel and that's ok?
        | r when (r.Substring(r.IndexOf(" "))).Trim().Length > 0 -> Some ((r.Substring(r.IndexOf(" "))).Trim())
        | _ -> None

    let (|Address|_|) (frame:string) =
        match frame.IndexOf(":") with
        | a when a < 1  -> None
        | _             -> Some (frame.Substring(0, frame.IndexOf(":")))

    let (|Sender|_|) (address:string) =
        match address.IndexOf(">") < 1 with
        | true  -> None
        | false -> Some (address.Substring(0, address.IndexOf(">")))

    let (|Destination|_|) (address:string) =
        match address.IndexOf(">") < 1 || address.IndexOf(",") < 1 with
        | true  -> None
        | false -> Some (address.Substring(address.IndexOf(">") + 1, address.IndexOf(",") - address.IndexOf(">") - 1))

    //Returns a list of the paths if parsed
    let (|Path|_|) (address:string) =
        match not (address.IndexOf(">") = -1) && address.IndexOf(",") > address.IndexOf(">") with
        | true  -> Some (address.Substring(address.IndexOf(",") + 1).Split(','))
        | false -> None

    let (|Information|_|) (frame:string) =
        match frame.IndexOf(":") < 1 with
        | true  -> None
        | false -> Some (frame.Substring(frame.IndexOf(":") + 1))
