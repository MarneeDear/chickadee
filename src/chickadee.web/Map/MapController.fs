namespace Map

open Saturn
open Giraffe
open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.AspNetCore.Http
open System.Text
open OtpNet
open Config
open Microsoft.Extensions.Logging

module Controller =

    let indexAction (ctx: HttpContext) = 
        let cnf = Controller.getConfig ctx
        let logger = ctx.GetLogger()

        task {
            //generate OTP to get the api token
            let otp =
                let secretKey : byte[] = Encoding.ASCII.GetBytes("SEED ME HELLO WORLD 12345!")
                let totp = new Totp(secretKey)
                let totpCode = totp.ComputeTotp()
                totpCode
            ctx.Session.SetString("OTP", otp)
            return Views.index otp cnf.clientId
        }

    let resource = controller {
        index indexAction
    }
