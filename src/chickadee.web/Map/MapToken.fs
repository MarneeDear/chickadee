module MapToken

open FSharp.Control.Tasks.ContextInsensitive
open Microsoft.Azure.Services.AppAuthentication
open Microsoft.AspNetCore.Http
open Giraffe

let GetTokenAsync (ctx:HttpContext) =
    task {
        match ctx.TryGetRequestHeader "X-API-Key" with
        | Some h ->
                    if ctx.Session.GetString("OTP") = h then
                        let tokenProvider = new AzureServiceTokenProvider()
                        let! accessToken = tokenProvider.GetAccessTokenAsync("https://atlas.microsoft.com/", null, ctx.RequestAborted)
                        return accessToken
                    else 
                        return System.String.Empty
        | None -> return System.String.Empty
    }
    