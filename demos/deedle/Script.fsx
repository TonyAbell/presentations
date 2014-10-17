
#load "Deedle.fsx"
#load "FSharp.Charting.fsx"


open System
open Deedle
open FSharp.Charting

open System
open Deedle
open FSharp.Charting

// Create from sequence of keys and sequence of values
let dates  = 
  [ DateTime(2013,1,1); 
    DateTime(2013,1,4); 
    DateTime(2013,1,8) ]
let values = 
  [ 10.0; 20.0; 30.0 ]
let first = Series(dates, values)

// Create from a single list of observations
Series.ofObservations
  [ DateTime(2013,1,1) => 10.0
    DateTime(2013,1,4) => 20.0
    DateTime(2013,1,8) => 30.0 ]


// Shorter alternative to 'Series.ofObservations'
series [ 1 => 1.0; 2 => 2.0 ]

// Create series with implicit (ordinal) keys
Series.ofValues [ 10.0; 20.0; 30.0 ]

/// Generate date range from 'first' with 'count' days
let dateRange (first:System.DateTime) count = 
  seq { for i in 0 .. (count - 1) -> first.AddDays(float i) }


/// Generate 'count' number of random doubles
let rand count = 
  let rnd = System.Random()
  seq { for i in 0 .. (count - 1) -> rnd.NextDouble() }

// A series with values for 10 days 
let second = Series(dateRange (DateTime(2013,1,1)) 10, rand 10)


let df1 = Frame(["first"; "second"], [first; second])

// The same as previously
let df2 = Frame.ofColumns ["first" => first; "second" => second]

// Transposed - here, rows are "first" and "second" & columns are dates
let df3 = Frame.ofRows ["first" => first; "second" => second]

// Create from individual observations (row * column * value)
let df4 = 
  [ ("Monday", "Tomas", 1.0); ("Tuesday", "Adam", 2.1)
    ("Tuesday", "Tomas", 4.0); ("Wednesday", "Tomas", -5.4) ]
  |> Frame.ofValues


// Assuming we have a record 'Price' and a collection 'values'
type Price = { Day : DateTime; Open : float }
let prices = 
  [ { Day = DateTime.Now; Open = 10.1 }
    { Day = DateTime.Now.AddDays(1.0); Open = 15.1 }
    { Day = DateTime.Now.AddDays(2.0); Open = 9.1 } ]

// Creates a data frame with columns 'Day' and 'Open'
let df5 = Frame.ofRecords prices


let msftCsv = Frame.ReadCsv(__SOURCE_DIRECTORY__ + "/data/stocks/MSFT.csv")
let fbCsv = Frame.ReadCsv(__SOURCE_DIRECTORY__ + "/data/stocks/FB.csv")

// Use the Date column as the index & order rows
let msftOrd = 
  msftCsv
  |> Frame.indexRowsDate "Date"
  |> Frame.sortRowsByKey

(*** define-output: plot1 ***)
// Create data frame with just Open and Close prices
let msft = msftOrd.Columns.[ ["Open"; "Close"] ]

// Add new column with the difference between Open & Close
msft?Difference <- msft?Open - msft?Close

// Do the same thing for Facebook
let fb = 
  fbCsv
  |> Frame.indexRowsDate "Date"
  |> Frame.sortRowsByKey
  |> Frame.sliceCols ["Open"; "Close"]
fb?Difference <- fb?Open - fb?Close

// Now we can easily plot the differences
Chart.Combine
  [ Chart.Line(msft?Difference |> Series.observations) 
    Chart.Line(fb?Difference |> Series.observations) ]

// Change the column names so that they are unique
let msftNames = ["MsftOpen"; "MsftClose"; "MsftDiff"]
let msftRen = msft |> Frame.indexColsWith msftNames

let fbNames = ["FbOpen"; "FbClose"; "FbDiff"]
let fbRen = fb |> Frame.indexColsWith fbNames

// Outer join (align & fill with missing values)
let joinedOut = msftRen.Join(fbRen, kind=JoinKind.Outer)

// Inner join (remove rows with missing values)
let joinedIn = msftRen.Join(fbRen, kind=JoinKind.Inner)

// Visualize daily differences on available values only
Chart.Rows
  [ Chart.Line(joinedIn?MsftDiff |> Series.observations) 
    Chart.Line(joinedIn?FbDiff |> Series.observations) ]

// Look for a row at a specific date
joinedIn.Rows.[DateTime(2013, 1, 2)]


// Get opening Facebook price for 2 Jan 2013
joinedIn.Rows.[DateTime(2013, 1, 2)]?FbOpen


// Get values for the first three days of January 2013
let janDates = [ for d in 2 .. 4 -> DateTime(2013, 1, d) ]
let jan234 = joinedIn.Rows.[janDates]

// Calculate mean of Open price for 3 days
jan234?MsftOpen |> Stats.mean

// Get values corresponding to entire January 2013
let jan = joinedIn.Rows.[DateTime(2013, 1, 1) .. DateTime(2013, 1, 31)] 



// Calculate means over the period
jan?FbOpen |> Stats.mean
jan?MsftOpen |> Stats.mean


let daysSeries = Series(dateRange DateTime.Today 10, rand 10)
let obsSeries = Series(dateRange DateTime.Now 10, rand 10)


// Fails, because current time is not present
try daysSeries.[DateTime.Now] with _ -> nan
try obsSeries.[DateTime.Now] with _ -> nan

// This works - we get the value for DateTime.Today (12:00 AM)
daysSeries.Get(DateTime.Now, Lookup.ExactOrSmaller)
// This does not - there is no nearest key <= Today 12:00 AM
try obsSeries.Get(DateTime.Today, Lookup.ExactOrSmaller)
with _ -> nan

let daysFrame = [ 1 => daysSeries ] |> Frame.ofColumns
let obsFrame = [ 2 => obsSeries ] |> Frame.ofColumns

// All values in column 2 are missing (because the times do not match)
let obsDaysExact = daysFrame.Join(obsFrame, kind=JoinKind.Left)

// All values are available - for each day, we find the nearest smaller
// time in the frame indexed by later times in the day
let obsDaysPrev = 
  (daysFrame, obsFrame) 
  ||> Frame.joinAlign JoinKind.Left Lookup.ExactOrSmaller

// The first value is missing (because there is no nearest 
// value with greater key - the first one has the smallest 
// key) but the rest is available
let obsDaysNext =
  (daysFrame, obsFrame) 
  ||> Frame.joinAlign JoinKind.Left Lookup.ExactOrGreater


joinedOut?Comparison <- joinedOut |> Frame.mapRowValues (fun row -> 
  if row?MsftOpen > row?FbOpen then "MSFT" else "FB")


joinedOut.GetColumn<string>("Comparison")
|> Series.filterValues ((=) "MSFT") |> Series.countValues
// [fsi:val it : int = 220]

joinedOut.GetColumn<string>("Comparison")
|> Series.filterValues ((=) "FB") |> Series.countValues


// Get data frame with only 'Open' columns
let joinedOpens = joinedOut.Columns.[ ["MsftOpen"; "FbOpen"] ]

// Get only rows that don't have any missing values
// and then we can safely filter & count
joinedOpens.RowsDense
|> Series.filterValues (fun row -> row?MsftOpen > row?FbOpen)
|> Series.countValues

let monthly =
  joinedIn
  |> Frame.groupRowsUsing (fun k _ -> DateTime(k.Year, k.Month, 1))


//monthly.Rows.[DateTime(2013,5,1), *] |> Stats.mean


monthly 
|> Frame.getNumericCols
|> Series.mapValues (Stats.levelMean fst)
|> Frame.ofColumns

