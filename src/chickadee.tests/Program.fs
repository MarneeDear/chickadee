﻿open Expecto
open System
open System.Reflection
open Microsoft.Data.Sqlite
open SimpleMigrations.DatabaseProvider
open SimpleMigrations
open SimpleMigrations.Console


[<EntryPoint>]
let main argv =

    //LOAD chickadee.migration and use the simple migrations console runner to create a new test database or migrate an existing one
    let mutable database_location : string = "DataSource=database.sqlite"
    if argv.Length > 0 then
        database_location <- argv.[0]
    Console.WriteLine database_location
    let assembly = Assembly.Load("chickadee.migrations") // Assembly.GetExecutingAssembly()
    use db = new SqliteConnection (database_location) //"DataSource=..\..\database\database.sqlite"
    //Drop database and recreate it
    System.IO.File.Delete("./database.sqlite")
    let provider = SqliteDatabaseProvider(db)
    let migrator = SimpleMigrator(assembly, provider)
    let consoleRunner = ConsoleRunner(migrator)
    consoleRunner.Run(Array.empty) |> ignore
    //END migrate database

    //RUN TESTS
    let writeResults = TestResults.writeNUnitSummary ("ChickTestResults.xml", "Expecto.Tests")
    let config = defaultConfig.appendSummaryHandler writeResults

    //TNC2MONRepositoryTests.ProcessTNC2RecordTests
    //|> runTests config

    //TNC2FormatTests.RawPacketTypeTests
    //|> runTests config

    //TNC2FormatTests.DataIdentifierTests
    //|> runTests config

    //DireWolfTests.WriteFramesTests
    //|> runTests config

    //APRSDataTests.APRSDataTests
    //|> runTests config

    //PositionReportTests.PositionReportTests
    //|> runTests config

    //MessageTests.MessageTests
    //|> runTests config

    //CommonTests.SymbolCodeTests
    //|> runTests config

    runTestsInAssembly config argv

    //runTestsInAssembly defaultConfig argv
    //Console.ReadKey() |> ignore
    //0