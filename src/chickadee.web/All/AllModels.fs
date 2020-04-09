namespace All

module Models =
    type AllReceived =
        {
            DateCreated : string
            RawPacket : string
            PacketType : string
            Error : string
        }

    type AllTransmitted = 
        {
            DateCreated : string
            RawPacket : string
            PacketType : string
            Transmitted : bool
        }