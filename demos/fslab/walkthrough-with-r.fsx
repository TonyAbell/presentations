//https://github.com/tpetricek/FsLab/blob/master/src/experiments/walkthrough-with-r/Tutorial.fsx
#load "FsLab.fsx"

open Deedle
open FSharp.Data
open MathNet.Numerics.LinearAlgebra
open FSharp.Charting

open RProvider
open RProvider.graphics
open RProvider.stats
open RProvider.``base``

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



// Create matrix from debts & vector from means
let debtsMat = debts |> Frame.fillMissingWith 0.0 |> Frame.toMatrix
let avgVect = debts |> Stats.mean |> Series.toVector

// Multiply debts per year by means
debtsMat * avgVect



// Combine three line charts and add a legend
Chart.Combine(
  [ Chart.Line(recent?Cyprus, Name="Cyprus")
    Chart.Line(recent?Malta, Name="Malta")
    Chart.Line(recent?Greece, Name="Greece") ])
  .WithLegend()



R.hist(debts.GetAllValues<float>())

//
let rdf = R.as_data_frame(R.cor(recent))
let cors = rdf.GetValue<Frame<string, string>>()
//
//
cors
|> Frame.stack
|> Frame.filterRowValues (fun row -> 
    row.GetAs<string>("Row") < row.GetAs<string>("Column") )
|> Frame.sortRowsBy "Value" ((*) -1.0)
|> Frame.take 5