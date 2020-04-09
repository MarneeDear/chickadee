namespace All

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine
    open Models

    let index (rxs:AllReceived list option) (txs:AllTransmitted list option) (rxsError:string option) (txsError:string option)  = 
        let content = [
            //for rx in rxs do
            //    encodedText rx.RawPacket

            //for tx in txs do
            //    encodedText tx.RawPacket
            section [_class "section"] [
                h1 [_class "title"] [encodedText "Received Frames"]
                table [_class "table"] [
                    thead [] [
                        th [] [encodedText "Date Created"]
                        th [] [encodedText "Raw Packet"]
                        th [] [encodedText "Packet Type"]
                        th [] [encodedText "Error"]
                    ]
                    tbody [] [
                        if rxs.IsSome then
                            for rx in rxs.Value do
                                tr [] [
                                    td [] [encodedText rx.DateCreated]
                                    td [] [encodedText rx.RawPacket]
                                    td [] [encodedText rx.PacketType]
                                    td [] [encodedText rx.Error]
                                ]
                    ]
                ]
            ]
            section [_class "section"] [
                h1 [_class "title"] [encodedText "Transmit Frames"]
                table [_class "table"] [
                    thead [] [
                        th [] [encodedText "Date Created"]
                        th [] [encodedText "Raw Packet"]
                        th [] [encodedText "Packet Type"]
                        th [] [encodedText "Transmitted?"]
                    ]
                    tbody [] [
                        if txs.IsSome then
                            for tx in txs.Value do
                                tr [] [
                                    td [] [encodedText tx.DateCreated]
                                    td [] [encodedText tx.RawPacket]
                                    td [] [encodedText tx.PacketType]
                                    td [] [encodedText (string tx.Transmitted)]
                                ]
                    ]
                ]
            ]

        ]
        App.layout content