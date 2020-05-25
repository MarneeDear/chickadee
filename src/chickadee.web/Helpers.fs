module Helpers

open Microsoft.AspNetCore.Http
open Giraffe
open Microsoft.AspNetCore.Antiforgery
open GiraffeViewEngine
open FSharp.Control.Tasks
open Thoth.Json.Net

///Creates a csrf token form input of the kind: <input type="hidden" name="TOKEN_NAME" value="TOKEN_VALUE" />
let csrfTokenInput (ctx: HttpContext) =
    let antiforgery = ctx.GetService<IAntiforgery>()
    let tokens = antiforgery.GetAndStoreTokens(ctx)
    input [ _name tokens.FormFieldName
            _value tokens.RequestToken
            _type "hidden" ]

///View helper for creating a form that implicitly inserts a CSRF token hidden form input.
let protectedForm ctx attrs children = form attrs (csrfTokenInput ctx :: children)  

[<CLIMutable>]
type UserSettings =
    {
        CallSign : string
        LocationLatitude : float
        LocationLongitude : float
    }

let emptyUserSettings () =
    {
        CallSign = "No call sign configured"
        LocationLatitude = 0.0
        LocationLongitude = 0.0 
    }

let getUserSettings () =
    task {
        try
            let! json = System.IO.File.ReadAllTextAsync("usersettings.json")        
            return Decode.Auto.fromString<UserSettings> json
        with
        | ex -> return Error ex.Message
    }

let setSettingsSession (ctx:HttpContext) (settings:UserSettings) =
    ctx.Session.SetString("CallSign", settings.CallSign)
    ctx.Session.SetString("LocationLatitude", string settings.LocationLatitude)
    ctx.Session.SetString("LocationLongitude", string settings.LocationLongitude)

let sessionCallSign (ctx:HttpContext) =
    ctx.Session.GetString("CallSign")

let sessionLocationLatitude (ctx:HttpContext) =
    ctx.Session.GetString("LocationLatitude")

let sessionLocationLongitude (ctx:HttpContext) =
    ctx.Session.GetString("LocationLongitude")