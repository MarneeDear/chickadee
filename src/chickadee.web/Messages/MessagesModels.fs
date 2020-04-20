namespace Messages

module Model =
    [<CLIMutable>]
    type Message =
        {
            Addressee       : string
            MessageText     : string
            MessageNumber   : string
        }