#load ".fake/build.fsx/intellisense.fsx"
open System
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment ()

//C:\Users\marnee\Dropbox\github\radio\aprs-projects\chickadee\publishRPi\publishCli

let servicePath = "./src/chickadee.service/" |> Fake.IO.Path.getFullName
let webPath = "./src/chickadee.web/" |> Fake.IO.Path.getFullName
let testPath = "./src/chickadee.tests/" |> Fake.IO.Path.getFullName
let migrationPath = "./src/chickadee.migrations/" |> Fake.IO.Path.getFullName
let cliPath = "./src/chickadee.cli/" |> Fake.IO.Path.getFullName
let cliPublishTo = "./publishRPi/publishCli" |> Fake.IO.Path.getFullName
let webPublishTo = "./publishRPi/publishWeb" |> Fake.IO.Path.getFullName
let servicePublishTo = "./publishRPi/publishService" |> Fake.IO.Path.getFullName
let migratePublishTo = "./publishRPi/publishMigration" |> Fake.IO.Path.getFullName

let runDotNet cmd workingDir =
    let result =
        DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

let openBrowser url =
    //https://github.com/dotnet/corefx/issues/10361
    Command.ShellCommand url
    |> CreateProcess.fromCommand
    |> CreateProcess.ensureExitCodeWithMessage "opening browser failed"
    |> Proc.run
    |> ignore

open Fake.IO.FileSystemOperators

Trace.trace (File.readAsString "build-art.txt")

Target.create "Clean" (fun _ ->
    //!! "src/**/bin"
    //++ "src/**/obj"
    //|> Shell.cleanDirs 
    runDotNet "clean" "./"
)

Target.create "Build" (fun _ ->
    //!! "src/**/*.*proj"
    //|> Seq.iter (runDotNet "build") //(DotNet.build id)
    runDotNet "build" ("." |> Fake.IO.Path.getFullName)
)

Target.create "RunWeb" (fun _ -> 
  let server = async {
    runDotNet "watch run" webPath |> ignore
    }

  let browser = async {
    Threading.Thread.Sleep 8000
    openBrowser "https://chickadee.local" |> ignore
  }

  [ server; browser]
  |> Async.Parallel
  |> Async.RunSynchronously
  |> ignore
)

Target.create "RunService" (fun _ -> 
    runDotNet "watch run" servicePath |> ignore
)

//Target.create "RunCLI" (fun _ ->
//    runDotNet "run" cliPath |> ignore
//)

open Fake.IO.Globbing.Operators
open System.Net

Target.create "Test" (fun _ -> 
    //DotNet.test (fun p -> p) infrastructureTestsPath
    runDotNet "run" testPath |> ignore
)

Target.create "MigrateUp" (fun _ -> 
    //DotNet.test (fun p -> p) infrastructureTestsPath
    runDotNet "run" migrationPath |> ignore
)

//Publish ALL to RPi
//dotnet publish -r linux-arm --self-contained -o ../../publish src/chickadee.cli
Target.create "PublishRPi" (fun _ ->
    //TODO compile to release, not debug first
    runDotNet (sprintf "publish -r linux-arm --self-contained -o %s" cliPublishTo) cliPath
    runDotNet (sprintf "publish -r linux-arm --self-contained -o %s" webPublishTo) webPath
    runDotNet (sprintf "publish -r linux-arm --self-contained -o %s" servicePublishTo) servicePath
    runDotNet (sprintf "publish -r linux-arm --self-contained -o %s" migratePublishTo) migrationPath
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "Test"
  ==> "All"

"Clean"
    ==> "Build"
    ==> "RunWeb"

"Clean"
    ==> "Build"
    ==> "RunService"

//"Clean"
//    ==> "Build"
//    ==> "RunCLI"

"Clean"
    ==> "Build"
    ==> "Test"
    ==> "PublishRPi"

Target.runOrDefault "All"
