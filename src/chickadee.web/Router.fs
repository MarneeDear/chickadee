module Router

open Saturn
open Giraffe.Core
open Giraffe.ResponseWriters
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration

let browser = pipeline {
    plug acceptHtml
    plug putSecureBrowserHeaders
    plug fetchSession
    set_header "x-pipeline-type" "Browser"
}

let defaultView = router {
    get "/" (htmlView Index.layout)
    get "/index.html" (redirectTo false "/")
    get "/default.html" (redirectTo false "/")
}

let validateApiKey (ctx : HttpContext) =
    let configuration = ctx.GetService<IConfiguration>()
    let apiKey = configuration.["TokenAPIKey"]
    match ctx.TryGetRequestHeader "X-API-Key" with
    | Some key -> apiKey.Equals key
    | None     -> false

//let accessDenied   = setStatusCode 401 >=> text "Access Denied"
//let requiresApiKey = authorizeRequest validateApiKey accessDenied

let browserRouter = router {
    not_found_handler (htmlView NotFound.layout) //Use the default 404 webpage
    pipe_through browser //Use the default browser pipeline

    forward "" defaultView //Use the default view

    //POSITION REPORTS
    //GET a list of messages
    //Will I store them in a database? SQL Lite?
    //POST a message to send with KISSUTIL
    //Store sent messages and give option to re-send
    forward "/position_reports" PositionReports.Controller.resource

    //Map
    forward "/map" Map.Controller.resource

    //RACE REPORTS
    forward "/race_reports" RaceReport.Controller.resource

    //MESSAGES
    forward "/messages" Messages.Controller.resource
    forward "/messages/add" Messages.Controller.resource

    //WEATHER REPORTS
    forward "/weather_reports" WeatherReport.Controller.resource

    //All received messages
    forward "/all" All.Controller.resource
}

//Other scopes may use different pipelines and error handlers

let api = pipeline {
     //plug acceptJson
     plug fetchSession
     //plug requiresApiKey     
     set_header "x-pipeline-type" "API"
}

let apiRouter = router {
     //error_handler (text "Api 404")
     pipe_through api
     forward "/token" (fun next ctx -> (text (MapToken.GetTokenAsync ctx).Result) next ctx)
     forward "/map_data" MapData.Controller.resource //(fun next ctx -> ((MapData.GetPositionReports ctx).Result) next ctx)
}

let appRouter = router {
    forward "/api" apiRouter
    forward "" browserRouter
}