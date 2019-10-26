namespace chickadee.core

module Message =

    type MessageText = private MessageText of string
    module MessageText =
        let create (m:string) =
            match (m.Trim()) with
            | m when m.Length <= 67    -> MessageText m //AX.25 field is 256 chars but the message has to accomodate the { for user defined messages
            | _                         -> MessageText (m.Substring(0, 67)) //or return None TODO or throw an exception?
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
    *)
    type Message =
        {
            Addressee : CallSign
            MessageText : MessageText
            MessageNumber : MessageNumber
        }
        override this.ToString() =
            sprintf ":%s:%s{%s" (CallSign.value this.Addressee) (MessageText.value this.MessageText) (MessageNumber.value this.MessageNumber)

    type Bulletin = 
        {
            TODO : string
        }

    type Announcement =
        {
            TODO2 : string
        }

    type MessageType =
        | Message of Message
        | Bulletin of Bulletin
        | Announcement of Announcement
