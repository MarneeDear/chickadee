#load ".fake/build.fsx/intellisense.fsx"
open System
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment ()

let servicePath = "./src/chickadee.service/" |> Fake.IO.Path.getFullName
let testPath = "./src/chickadee.tests/" |> Fake.IO.Path.getFullName

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
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "Run" (fun _ -> 
  let server = async {
    runDotNet "watch run" servicePath |> ignore
    }

  let browser = async {
    Threading.Thread.Sleep 8000
    openBrowser "http://chickadee.local:8085" |> ignore
  }

  [ server; browser]
  |> Async.Parallel
  |> Async.RunSynchronously
  |> ignore
)

open Fake.IO.Globbing.Operators
open System.Net

Target.create "Test" (fun _ -> 
    //DotNet.test (fun p -> p) infrastructureTestsPath
    runDotNet "run" testPath |> ignore
)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "Test"
  ==> "All"

"Clean"
    ==> "Build"
    ==> "Run"

Target.runOrDefault "All"
