namespace All

module Views =
    open Microsoft.AspNetCore.Http
    open Giraffe.GiraffeViewEngine

    let index = 
        let content = [
            encodedText "There are so many messages"
        ]
        App.layout content