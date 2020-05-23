module MessageTests

open Expecto
open chickadee.core.Common
open chickadee.core.Message

[<Literal>]
let MESSAGE = "TESTING MESSAGING"
let CALLSIGN = "KG7SIO"
let MSGNUM = "00999"

[<Tests>]
let MessageTests =
    testList "Message Format Tests" [
        testCase "Can create Message" <| fun _ ->
            let msg = {
                Addressee = (CallSign.create CALLSIGN).Value
                MessageText = (MessageText.create MESSAGE).Value
                MessageNumber = (MessageNumber.create MSGNUM)
            }
            Expect.equal (msg.ToString()) (sprintf ":KG7SIO   :TESTING MESSAGING{%s" MSGNUM) "Message not in expected format"
        testCase "Can create Message Acknowledgement" <| fun _ ->
            let ack : MessageAcknowledgement = {
                Addressee = (CallSign.create CALLSIGN).Value
                MessageNumber = (MessageNumber.create MSGNUM).Value
            }
            Expect.equal (ack.ToString()) (sprintf ":KG7SIO   :ack%s" MSGNUM) "Message Acknowledgement not in expetected format"
        testCase "Can create Message Rejection" <| fun _ ->
            let rej : MessageRejection = {
                Addressee = (CallSign.create CALLSIGN).Value
                MessageNumber = (MessageNumber.create MSGNUM).Value
            }
            Expect.equal (rej.ToString()) (sprintf ":KG7SIO   :rej%s" MSGNUM) "Message Rejection not in expetected format"
        testCase "Can create Bulletin" <| fun _ ->
            let bul : Bulletin = {
                BulletinId = (MessageID.create "1").Value
                BulletinText = (MessageText.create MESSAGE).Value
            }
            Expect.equal (bul.ToString()) (sprintf ":BLN1     :%s" MESSAGE) "Bulletin not in expected format"
        testCase "Can create Announcement" <| fun _ ->
            let bul : Announcement = {
                AnnouncementId = (MessageID.create "Q").Value
                AnnouncementText = (MessageText.create MESSAGE).Value
            }
            Expect.equal (bul.ToString()) (sprintf ":BLNQ     :%s" MESSAGE) "Announcement not in expected format"

    ]

