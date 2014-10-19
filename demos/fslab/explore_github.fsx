#load "FsLab.fsx"

open Deedle
open FSharp.Data
open FSharp.Charting
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics
open RProvider

//https://developer.github.com/v3/repos/#list-organization-repositories
//http://www.navision-blog.de/blog/2014/10/14/retrieving-github-download-counts/
type GitHubReleases = JsonProvider<"https://api.github.com/repos/fsprojects/Paket/releases">
 
 
let downloadCounts = 
    [for release in GitHubReleases.GetSamples() do
        for asset in release.Assets do
            if asset.Name = "paket.exe" then
                yield release.Name,asset.DownloadCount]
 

Chart.Bar downloadCounts




type fsprojectsRepos = JsonProvider<"https://api.github.com/orgs/fsprojects/repos">

fsprojectsRepos.GetSamples() |> Seq.map( fun f -> f.LanguagesUrl) |> Seq.toArray


type gitHubLangs = JsonProvider<"https://api.github.com/repos/fsprojects/Vulpes/languages">




Http.RequestString("https://api.github.com/repos/fsprojects/Vulpes/languages", headers= [("User-Agent","demo")])


let getProjectFSharpByteCount (url:string) =    
    try 
        float (gitHubLangs.Load(url).F)
    with _ -> 0.0
 

let reposAndSize = 
        fsprojectsRepos.GetSamples() 
                |> Seq.toArray
                |> Array.Parallel.map( fun f -> f,getProjectFSharpByteCount f.LanguagesUrl) 
System.DateTime(1970,1,1,0,0,0,System.DateTimeKind.Utc)
      .AddSeconds((1413690253.0))
      .ToLocalTime()
      .ToShortTimeString()

let sizeSeries = 
    reposAndSize 
    |> Seq.map(fun (f,s)-> f.Name,(float)s)
    |> Series.ofObservations             


sizeSeries 
    |> Series.sort
    //|> Series.rev
    |> Series.skipLast 1
    |> Series.takeLast 10
    
    |> Chart.Bar

open System
open RDotNet
open RProvider
open RProvider.``base``
open RProvider.graphics
open RProvider.stats
// Random number generator
let rng = Random()
let rand () = rng.NextDouble()

// Generate fake X1 and X2 
let X1s = [ for i in 0 .. 9 -> 10. * rand () ]
let X2s = [ for i in 0 .. 9 -> 5. * rand () ]

// Build Ys, following the "true" model
let Ys = [ for i in 0 .. 9 -> 5. + 3. * X1s.[i] - 2. * X2s.[i] + rand () ]


let dataset =
    namedParams [
        "Y", box Ys;
        "X1", box X1s;
        "X2", box X2s; ]
    |> R.data_frame

let result = R.lm(formula = "Y~X1+X2", data = dataset)

let coefficients = result.AsList().["coefficients"].AsNumeric()
let residuals = result.AsList().["residuals"].AsNumeric()

let summary = R.summary(result)
summary.AsList().["r.squared"].AsNumeric()


 
R.plot result
 

let basicStats = 
        series [
          "Mean" => round (Stats.mean sizeSeries)
          "Max" => Option.get (Stats.max sizeSeries)
          "Min" => Option.get (Stats.min sizeSeries)
          "Median" => Stats.median sizeSeries 
          "+/-" => Stats.stdDev sizeSeries ]


