module Console

open System

let log =
    let lockObj = obj()
    fun color s ->
        lock lockObj (fun _ ->
            Console.ForegroundColor <- color
            printfn "%s" s
            Console.ResetColor())

let complete    = log ConsoleColor.Magenta
let ok          = log ConsoleColor.Green
let info        = log ConsoleColor.Blue
let warn        = log ConsoleColor.Yellow
let error       = log ConsoleColor.Red
