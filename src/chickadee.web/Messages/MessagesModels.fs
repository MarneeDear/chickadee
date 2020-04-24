namespace Messages

module Model =
    [<CLIMutable>]
    type Message =
        {
            Addressee       : string
            MessageText     : string
            MessageNumber   : string
        }

    [<CLIMutable>]
    type ReceivedMessage =
        {
            DateCreated : string
            RawPacket : string
            Addressee : string
            MessageText : string
            MessageNumber : string
            Error : string
        }

    [<CLIMutable>]
    type TransmittedMessage =
        {
            DateCreated : string
            RawPacket : string
            Addressee : string
            MessageText : string
            MessageNumber : string
            Error : string
            Transmitted : bool
        }
