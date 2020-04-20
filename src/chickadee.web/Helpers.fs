module Helpers

open Microsoft.AspNetCore.Http
open Giraffe
open Microsoft.AspNetCore.Antiforgery
open GiraffeViewEngine

///Creates a csrf token form input of the kind: <input type="hidden" name="TOKEN_NAME" value="TOKEN_VALUE" />
let csrfTokenInput (ctx: HttpContext) =
    let antiforgery = ctx.GetService<IAntiforgery>()
    let tokens = antiforgery.GetAndStoreTokens(ctx)
    input [ _name tokens.FormFieldName
            _value tokens.RequestToken
            _type "hidden" ]

///View helper for creating a form that implicitly inserts a CSRF token hidden form input.
let protectedForm ctx attrs children = form attrs (csrfTokenInput ctx :: children)  
