#load "FsLab.fsx"
//https://github.com/tpetricek/FsLab/blob/master/src/experiments/walkthrough/Tutorial.fsx
open Deedle
open FSharp.Data
open FSharp.Charting
open MathNet.Numerics.LinearAlgebra


// Get countries in the Euro area
let wb = WorldBankData.GetDataContext()
let countries = wb.Regions.``Euro area``

// Get a frame with debts as a percentage of GDP 
let debts = 
  [ for c in countries.Countries ->
      let debts = c.Indicators.``Central government debt, total (% of GDP)``
      c.Name => series debts ] |> frame

let recent = debts.Rows.[2005 ..]

recent
|> Stats.mean
|> Series.sort
|> Series.rev
|> Series.take 4
|> round

let debtsMat = debts |> Frame.fillMissingWith 0.0 |> Frame.toMatrix
let avgVect = debts |> Stats.mean |> Series.toVector

debtsMat * avgVect



Chart.Combine(
  [ Chart.Line(recent?Cyprus, Name="Cyprus")
    Chart.Line(recent?Malta, Name="Malta")
    Chart.Line(recent?Greece, Name="Greece") ])
  .WithLegend()
