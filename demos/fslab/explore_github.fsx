#load "FsLab.fsx"


open Deedle
open FSharp.Data
open FSharp.Data
open FSharp.Charting
open System.Collections.Generic
open System.Collections.Specialized
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics
open RProvider
open System.Linq
//https://developer.github.com/v3/repos/#list-organization-repositories
//http://www.navision-blog.de/blog/2014/10/14/retrieving-github-download-counts/

[<Literal>]
let commitsUrl = "https://api.github.com/repos/fsprojects/fsharpx/commits"


type GitHubReleases = JsonProvider< "https://api.github.com/repos/fsprojects/Paket/releases" >

let downloadCounts = 
    [ for release in GitHubReleases.GetSamples() do
          for asset in release.Assets do
              if asset.Name = "paket.exe" then 
                  yield release.Name, asset.DownloadCount ]

Chart.Bar downloadCounts

type gitRepos = JsonProvider< "https://api.github.com/orgs/fsprojects/repos" >


gitRepos.GetSamples()
|> Seq.map (fun f -> f.CommitsUrl)
|> Seq.toArray

type gitPull = JsonProvider< "https://api.github.com/repos/fsprojects/fsharpx/pulls" >

type gitCommit = JsonProvider< commitsUrl >

let fsProjectsRepos = 
    gitRepos.Load("https://api.github.com/orgs/fsprojects/repos")

let fsProjectsPulls = 
    fsProjectsRepos
    |> Array.map (fun f -> f.PullsUrl)
    |> Array.map (fun f -> f.Substring(0, f.IndexOf('{')))
    |> Array.Parallel.map (fun f -> gitPull.Load(f))

fsProjectsPulls.[0]

type gitHubLangs = JsonProvider< "https://api.github.com/repos/fsprojects/Vulpes/languages" >

let RateLimitReset t = 
    System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)
          .AddSeconds((t)).ToLocalTime().ToShortTimeString()

let gitRequest r = 
    Http.Request
        (r, 
         headers = [ ("User-Agent", "demo")
                     ("Authorization", "Basic ") ])

let sampleLinkHeader = 
    "<https://api.github.com/repositories/977333/commits?page=55>; rel=\"next\", <https://api.github.com/repositories/977333/commits?page=1>; rel=\"first\", <https://api.github.com/repositories/977333/commits?page=53>; rel=\"prev\""

let (|Next|_|) (s : string) = 
    let parts = s.Split(';')
    if parts.[1].Trim() = "rel=\"next\"" then 
        Some(parts.[0].TrimStart('<').TrimEnd('>'))
    else None

let nextLinkUrl (link : string) = 
    link.Split(',')
    |> Array.map (fun f -> 
           match f with
           | Next(l) -> Some(l)
           | _ -> None)
    |> Array.choose (fun f -> f)
    |> function 
    | [| v |] -> Some(v)
    | _ -> None

nextLinkUrl sampleLinkHeader

let rec getRequestAllPages (pages) (r : string) = 
    let request = gitRequest r
    
    let body = 
        request.Body |> function 
        | Text(str) -> str
        | _ -> ""
    if request.Headers.ContainsKey("Link") then 
        match nextLinkUrl (request.Headers.Item("Link")) with
        | Some(link) -> body :: getRequestAllPages pages link
        | _ -> body :: pages
    else body :: pages

getRequestAllPages [] 
    "https://api.github.com/repos/TonyAbell/presentations/commits"


let getAllCommits url = 
     getRequestAllPages []  url
    |> Seq.toArray
    |> Array.Parallel.map (fun f -> gitCommit.Parse f)
    |> Array.Parallel.collect (fun f -> f)

let getPowerPackCommits = 
    getAllCommits "https://api.github.com/repos/fsprojects/powerpack/commits"
    

let getFsharxCommits = 
    getRequestAllPages [] 
        "https://api.github.com/repos/fsprojects/fsharpx/commits"
    |> Seq.toArray
    |> Array.Parallel.map (fun f -> gitCommit.Parse f)
    |> Array.Parallel.collect (fun f -> f)

let months = [ for d in 1 .. 50 -> System.DateTime.Today.AddMonths(-d) ] 
               |> List.sort

let days = [ for d in 1.0 .. 365.0 -> System.DateTime.Today.AddDays(-d) ] 
               |> List.sort

//let dic = new System.Collections.Generic.Dictionary<System.DateTime,int>()

let genKeyValue (dic:System.Collections.Generic.Dictionary<System.DateTime,int>) (k,v) : System.Collections.Generic.Dictionary<System.DateTime,int> =
    match dic.ContainsKey k with
        | true ->  dic.[k] <- dic.[k] + v
                   dic
        | false ->  dic.Add(k,v)
                    dic
    
//let commitSeries = 
//    getFsharxCommits
//    |> Seq.map (fun f -> f.Commit.Committer.Date, 1)                         
//    |> Seq.fold genKeyValue (new System.Collections.Generic.Dictionary<System.DateTime,int>())  
//    |> Seq.map(fun f-> f.Key,f.Value)
//    |> Series.ofObservations     
//    |> Series.sortByKey 
//    //|> Series.resample days Direction.Backward
//    |> Series.resampleInto months Direction.Forward (fun k s -> if s.IsEmpty then 0 else s |> Series.reduceValues (+) )
//    |> Chart.Line

let commitsToSeries (commits:JsonProvider<commitsUrl>.Root[]) =
        commits
        |> Seq.map (fun f -> f.Commit.Committer.Date, 1)                         
        |> Seq.fold genKeyValue (new System.Collections.Generic.Dictionary<System.DateTime,int>())  
        |> Seq.map(fun f-> f.Key,f.Value)
        |> Series.ofObservations     
        |> Series.sortByKey 
let sampleByMonth series =
    series |> Series.resampleInto months Direction.Forward (fun k s -> if s.IsEmpty then 0 else s |> Series.reduceValues (+) )

let temp = 
    [ "Fsharx" => sampleByMonth (commitsToSeries getFsharxCommits);
      "PowerPack" => sampleByMonth (commitsToSeries getPowerPackCommits)] 
    |> frame

let fsProjectsCommits = gitRepos.GetSamples()
                            |> Array.map (fun f -> f.Name,f.CommitsUrl)
                            |> Array.map (fun (n,u) -> n,u.Substring(0,u.IndexOf('{')))                            
                            |> Array.map (fun (n,url) -> n, getAllCommits url)
                            

fsProjectsCommits |> Seq.map(fun (n,c) -> n, sampleByMonth (commitsToSeries c) )

fsProjectsCommits |> Array.map(fun (n,c) -> n,sampleByMonth (commitsToSeries c))
                  |> Array.map(fun (n,s) -> Chart.Line(s,Name=n))
                  |> Chart.Combine

Chart.Combine
  [ Chart.Line(temp?Fsharx |> Series.observations,Name="Fsharx") 
    Chart.Line(temp?PowerPack |> Series.observations,Name="PowerPack") ]

commitsToSeries getFsharxCommits
commitsToSeries getPowerPackCommits
//
//
//    |> Series.sortByKey   
//    |> Series.groupInto (fun k v -> new System.DateTime(k.Year,k.Month,k.Day)) (fun k s -> 
//           s
//           |> Series.observations
//           |> Seq.map (snd)
//           |> Seq.reduce (+))
//    |> Chart.Bar



(gitRequest "https://api.github.com/user").Headers
|> fun f -> f.Item("X-RateLimit-Reset"), f.Item("X-RateLimit-Remaining")
|> fun f -> 
    f
    |> fst
    |> (float)
    |> RateLimitReset, f |> snd
("https://api.github.com/repos/fsprojects/fsharpx/commits" |> gitRequest)
Http.RequestString
    ("https://api.github.com/repos/fsprojects/Vulpes/languages", 
     headers = [ ("User-Agent", "demo") ])
Http.RequestString
    ("https://api.github.com/repos/fsharp/pulls", 
     headers = [ ("User-Agent", "demo") ])

let getProjectFSharpByteCount (url : string) = 
    try 
        float (gitHubLangs.Load(url).F)
    with _ -> 0.0

let reposAndSize = 
    fsprojectsRepos.GetSamples()
    |> Seq.toArray
    |> Array.Parallel.map (fun f -> f, getProjectFSharpByteCount f.LanguagesUrl)

let sizeSeries = 
    reposAndSize
    |> Seq.map (fun (f, s) -> f.Name, (float) s)
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
let rand() = rng.NextDouble()

// Generate fake X1 and X2 
let X1s = 
    [ for i in 0..9 -> 10. * rand() ]

let X2s = 
    [ for i in 0..9 -> 5. * rand() ]

// Build Ys, following the "true" model
let Ys = 
    [ for i in 0..9 -> 5. + 3. * X1s.[i] - 2. * X2s.[i] + rand() ]

let dataset = 
    namedParams [ "Y", box Ys
                  "X1", box X1s
                  "X2", box X2s ]
    |> R.data_frame

let result = R.lm (formula = "Y~X1+X2", data = dataset)
let coefficients = result.AsList().["coefficients"].AsNumeric()
let residuals = result.AsList().["residuals"].AsNumeric()
let summary = R.summary (result)

summary.AsList().["r.squared"].AsNumeric()
R.plot result

let basicStats = 
    series [ "Mean" => round (Stats.mean sizeSeries)
             "Max" => Option.get (Stats.max sizeSeries)
             "Min" => Option.get (Stats.min sizeSeries)
             "Median" => Stats.median sizeSeries
             "+/-" => Stats.stdDev sizeSeries ]
