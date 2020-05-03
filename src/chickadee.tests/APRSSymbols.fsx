#r "C:\\Users\\marnee\\.nuget\\packages\\fsharp.data\\3.3.3\\lib\\netstandard2.0\\FSharp.Data.dll"
open FSharp.Data

type APRSSymbols = FSharp.Data.CsvProvider<"../../data/primarysymbols.tsv">

let symbols = new APRSSymbols()

for row in symbols.Rows do
    if row.CODE = "/!" then
        printfn "CODE %s DSTCALL %s DESCRIPTION %s" row.CODE row.DSTCALL row.DESCRIPTION

let home = 
    symbols.Rows |> Seq.find (fun r -> r.CODE = "/!")

