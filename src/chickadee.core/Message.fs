namespace chickadee.core

(*
See APRS 101 14 MESSAGES, BULLETINS AND ANNOUNCEMENTS
APRS messages, bulletins and announcements are packets containing free
format text strings, and are intended to convey human-readable information.
A message is intended for reception by a single specified recipient, and an
acknowledgement is usually expected. Bulletins and announcements are
intended for reception by multiple recipients, and are not acknowledged.
Messages An APRS message is a text string with a specified addressee. The addressee
is a fixed 9-character field (padded with spaces if necessary) following the :
Data Type Identifier. The addressee field is followed by another :, then the
text of the message.

The message text may be up to 67 characters long, and may contain any
printable ASCII characters except |, ~ or {.
A message may also have an optional message identifier, which is appended
to the message text. The message identifier consists of the character {
followed by a message number (up to 5 alphanumeric characters, no spaces)
to identify the message.

Messages without a message identifier are not to be acknowledged.
Messages with a message identifier are intended to be acknowledged by the
addressee. The sending station will repeatedly send the message until it
receives an acknowledgement, or it is canceled, or it times out.
*)
module Message =

    type MessageText = private MessageText of string
    module MessageText =
        let create (m:string) =
            match (m.Trim()) with
            | m when m.Length <= 67    -> MessageText m 
            | _                        -> MessageText (m.Substring(0, 67)) //TODO or return None TODO or throw an exception?
        let value (MessageText m) = m

    type MessageNumber = private MessageNumber of string
    module MessageNumber =
        let create (n:string) =
            match (n.Trim()) with
            | n when n.Length <= 5 -> MessageNumber n
            | _ -> MessageNumber (n.Substring(0, 5)) //Or fail with None?
        let value (MessageNumber n) = n


    (*            
    Message Format
            | : | Addressee | : | Message Text  | Message ID | Message Number (xxxxx)
    bytes   | 1 |    9      | 1 |    0-67       |     {      |      1-5
    :KG7SIO___:HELLO WORLD{1111
    See APRS 101 14 MESSAGES, BULLETINS AND ANNOUNCEMENTS
    *)
    type Message =
        {
            Addressee       : CallSign
            MessageText     : MessageText
            MessageNumber   : MessageNumber
        }
        override this.ToString() =
            sprintf ":%s:%s{%s" (CallSign.value this.Addressee) (MessageText.value this.MessageText) (MessageNumber.value this.MessageNumber)

    //TODO
    type Bulletin = 
        {
            TODO : string
        }

    //TODO
    type Announcement =
        {
            TODO2 : string
        }

    type MessageType =
        | Message of Message
        | Bulletin of Bulletin
        | Announcement of Announcement
