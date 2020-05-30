module Config

open chickadee.core

//type UserSettings = {
//    CallSign : CallSign option
//    Location : float * float
//}

type Config = {
    connectionString : string
    tokenApiKey : string
    clientId : string
    otpSeed : string
}