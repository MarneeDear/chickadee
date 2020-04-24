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

    type MessageID = private MessageID of string
    module MessageID =
        let create (n:string) =
            match n with 
            | n when n.Length = 1 -> Some (MessageID n)
            | _ -> None
        let value (MessageID n) = n

    type MessageText = private MessageText of string
    module MessageText =
        let create (m:string) =
            match (m.Trim()) with
            | m when m.Length < 68 && not (m.Contains("|")) && not (m.Contains("~")) -> Some (MessageText m)
            | _ -> None 
        let value (MessageText m) = m

    type MessageNumber = private MessageNumber of string
    module MessageNumber =
        let create (n:string) =
            match (n.Trim()) with
            | n when n.Length <= 5 -> Some (MessageNumber n)
            | _ -> None //MessageNumber (n.Substring(0, 5)) //Or fail with None?
        let value (MessageNumber n) = n.PadLeft(5, '0')


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
            sprintf ":%-9s:%s{%s" (CallSign.value this.Addressee) (MessageText.value this.MessageText) (MessageNumber.value this.MessageNumber)

    (*
    Message Acknowledgement
    A message acknowledgement is similar to a message, except that the message
    text field contains just the letters ack, and this is followed by the Message
    Number being acknowledged.

    Message Acknowledgement Format
    | : | Addressee | : | ack | Message |
                                  No
                                xxxxx
    | 1|     9      | 1 | 3   |   1-5   |
    
    Example    :KB2ICI-14:ack003

    *)
    type MessageAcknowledgement =
        {
            Addressee : CallSign
            MessageNumber : MessageNumber
        }
        override this.ToString() =
            sprintf ":%-9s:ack%s" (CallSign.value this.Addressee) (MessageNumber.value this.MessageNumber)

    (*
    Message Rejection If a station is unable to accept a message, it can send a rej message instead
    of an ack message:

    | : | Addressee | : | rej | Message No |
    | 1 |     9     | 1 |  3  |   1-5      |

    Example
    :KB2ICI-14:rej003

    *)
    type MessageRejection =
        {
            Addressee : CallSign
            MessageNumber : MessageNumber
        }
        override this.ToString() =
            sprintf ":%-9s:rej%s" (CallSign.value this.Addressee) (MessageNumber.value this.MessageNumber)

    (*
    General Bulletins 
    
    General bulletins are messages where the addressee consists of the letters
    BLN followed by a single-digit bulletin identifier, followed by 5 filler spaces.
    General bulletins are generally transmitted a few times an hour for a few
    hours, and typically contain time sensitive information (such as weather
    status).
    Bulletin text may be up to 67 characters long, and may contain any printable
    ASCII characters except | or ~.

    | : | BLN | Bulletin ID | _____ | : | Bulletin Text |
    | 1 |  3  |     1       |   5   | 1 |    0-67       |

    ---- is 5 filler spaces

    Example
    :BLN3_____:Snow expected in Tampa RSN
    
    *)

    type Bulletin = 
        {
            BulletinId : MessageID
            BulletinText : MessageText
        }
        override this.ToString() =
            sprintf ":BLN%s%5s:%s" (MessageID.value this.BulletinId) System.String.Empty (MessageText.value this.BulletinText)

    (*
    Announcements 
    Announcements are similar to general bulletins, except that the letters BLN
    are followed by a single upper-case letter announcement identifier.
    Announcements are transmitted much less frequently than bulletins (but
    perhaps for several days), and although possibly timely in nature they are
    usually not time critical.
    Announcements are typically made for situations leading up to an event, in
    contrast to bulletins which are typically used within the event.
    Users should be alerted on arrival of a new bulletin or announcement.
    
    | : | BLN | Announcement Id | _____ | : | Announcement Text |
    | 1 |  3  |        1        |   5   | 1 |     0-67          |

    Example
    :BLNQ_____:Mt St Helen digi will be QRT this weekend

    *)

    type Announcement =
        {
            AnnouncementId : MessageID
            AnnouncementText : MessageText
        }
        override this.ToString() =
            sprintf ":BLN%s%5s:%s" (MessageID.value this.AnnouncementId) System.String.Empty (MessageText.value this.AnnouncementText)

    type MessageFormat =
        | Message of Message
        | MessageAcknowledgement of MessageAcknowledgement
        | MessageRejection of MessageRejection
        | Bulletin of Bulletin
        | Announcement of Announcement
        override this.ToString() =
            match this with
            | Message m -> m.ToString()
            | MessageAcknowledgement m -> m.ToString()
            | MessageRejection m -> m.ToString()
            | Bulletin m -> m.ToString()
            | Announcement m -> m.ToString()
