#r @"packages/FAKE/tools/FakeLib.dll"

#load @"packages\FsReveal\fsreveal\fsreveal.fsx"

open FsReveal
open Fake
open System.IO

let outDir = "output"
let inputDir = "content"

Target "Clean" (fun _ ->
    CleanDirs [outDir]
)

let generateFor (file:FileInfo) =
    let rec tryGenerate trials =
        try
            let outputFileName = file.Name.Replace(file.Extension,".html")
            match file.Extension with   
            | ".md" ->  FsReveal.GenerateOutputFromMarkdownFile outDir outputFileName file.FullName
            | ".fsx" -> FsReveal.GenerateOutputFromScriptFile outDir outputFileName file.FullName
            | _ -> ()
        with 
        | exn when trials > 0 -> tryGenerate (trials - 1)
        | exn -> tracefn "Could not generate slides for %s" file.FullName

    tryGenerate 3

Target "GenerateSlides" (fun _ ->
    !! "content/*.md"
    |> Seq.map fileInfo
    |> Seq.iter generateFor
)

Target "KeepRunning" (fun _ ->
    use watcher = new FileSystemWatcher(DirectoryInfo(inputDir).FullName,"*.*")
    watcher.EnableRaisingEvents <- true
    watcher.Changed.Add(fun e -> fileInfo e.FullPath |> generateFor)
    watcher.Created.Add(fun e -> fileInfo e.FullPath |> generateFor)
    watcher.Renamed.Add(fun e -> fileInfo e.FullPath |> generateFor)

    traceImportant "Waiting for slide edits. Press any key to stop."

    System.Console.ReadKey() |> ignore

    watcher.EnableRaisingEvents <- false
    watcher.Dispose()
)

"Clean"
  ==> "GenerateSlides"
  ==> "KeepRunning"

RunTargetOrDefault "GenerateSlides"

