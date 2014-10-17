- title : Fsharp Data Science and R
- description : Introduction to Fsharp Data Science
- author : Tony Abell
- theme : Night
- transition : default

***

### What is Data Science ? test

- Data Access
- Data Manipulation 
- Basic Statistics 
- Machine Learning

***

### Data Access

- SQL
- Type Providers 

***

### Type Providerss

- Gives typed access to unstructured data
- [Fsharp Data](http://fsharp.github.io/FSharp.Data/)
- Azure Type Providers
- AWS Type Providers 


---

###  HTML Type Provider 

[Link](http://fsharp.github.io/FSharp.Data/library/HtmlProvider.html)

    type MarketDepth = HtmlProvider<"../data/MarketDepth.htm">
    let mrktDepth = MarketDepth.Load("http..").Tables.Table1
    let firstRow = mrktDepth.Rows |> Seq.head
    let settlementDate = firstRow.``Settlement Day``
    let acceptedBid = firstRow.``Accepted Bid Vol``
    let acceptedOffer = firstRow.``Accepted Offer Vol``

---

###  World Bank Type Provider
[Link](http://fsharp.github.io/FSharp.Data/library/WorldBank.html)
-The World Bank is an international organization that provides financial and technical assistance to developing countries around the world.
    
    let data = WorldBankData.GetDataContext()
    data
          .Countries.``United Kingdom``
          .Indicators.``School enrollment, tertiary (% gross)``
        |> Seq.maxBy fst

---

###  Freebase Type Provider
[Link](http://fsharp.github.io/FSharp.Data/library/Freebase.html)

    let data = FreebaseData.GetDataContext()
    let elements = data.``Science and Technology``.Chemistry.``Chemical Elements``
    let all = elements |> Seq.toList
    printfn "Elements found: %d" (Seq.length all)
    let hydrogen = elements.Individuals.Hydrogen
    printfn "Atominc number: %A" hydrogen.``Atomic number``

---

###  Cvs Type Provider
[CVS Type Provider](http://fsharp.github.io/FSharp.Data/library/CsvProvider.html)
    
    type Stocks = CsvProvider<"../data/MSFT.csv">
    let msft = Stocks.Load("http://ichart.finance.yahoo.com/table.csv?s=MSFT")

    let firstRow = msft.Rows |> Seq.head
    let lastDate = firstRow.Date
    let lastOpen = firstRow.Open

---

###  Hive Type Provider

[Hive Type Provider](http://fsprojects.github.io/FSharp.Data.HiveProvider/tutorial.html)
    
    let dsn = "Sample Hortonworks Hive DSN; pwd=hadoop"
    type Conn = Hive.HiveTypeProvider<dsn, DefaultMetadataTimeout=1000>
    let context = Conn.GetDataContext()

    let query = hiveQuery {for row in context.sample_07 do
                       where (row.salary ?< 20000)
                       select row.description}

---

###  SQL Type Providers
- [Generated](http://fsprojects.github.io/SQLProvider/) 
- [Ereased](http://fsprojects.github.io/FSharp.Data.SqlClient/)

---

###  SQL Providers

    type sql = SqlDataProvider< 
              ConenctionString = @"Data Source=F:\sqlite\northwindEF.db ;Version=3",
              DatabaseVendor = Common.DatabaseProviderTypes.SQLITE,
              ResolutionPath = @"F:\sqlite\3",
              IndividualsAmount = 1000,
              UseOptionTypes = true >
    let ctx = sql.GetDataContext()

    // pick individual entities from the database 
    let christina = ctx.``[main].[Customers]``.Individuals.``As ContactName``.``BERGS, Christina Berglund``

    // directly enumerate an entity's relationships, 
    // this creates and triggers the relevant query in the background
    let christinasOrders = christina.FK_Orders_0_0 |> Seq.toArray


---

###  SQL Client (Ereased)

    [<Literal>]
    let query = "
    SELECT TOP(@TopN) FirstName, LastName, SalesYTD 
    FROM Sales.vSalesPerson
    WHERE CountryRegionName = @regionName AND SalesYTD > @salesMoreThan 
    ORDER BY SalesYTD" 

    type SalesPersonQuery = SqlCommandProvider<query, connectionString>
    let cmd = new SalesPersonQuery()

    cmd.AsyncExecute(TopN = 3L, regionName = "United States", salesMoreThan = 1000000M) 
    |> Async.RunSynchronously



---



***

### Manipulation 
- Deedle
- [IfSharp](https://github.com/BayardRock/IfSharp) 

---

### Deedle

- [Deedle](http://bluemountaincapital.github.io/Deedle/)

- [FsLab](https://github.com/tpetricek/FsLab)

- F# R type provider for interoperating with R 

- F# Charting for building interactive charts 

- F# Data with data-access with F# type providers 

- Math.NET Numerics for writing numerical calculations



---

### IfSharp

- [Line](https://github.com/BayardRock/IfSharp)


---

***

### Machine Learning  
- Supervised 
- Unsupervised 

---


###  Accord.Net

- Support Vector Machines
- Decision Trees
- Naive Bayesian models
- K-means

---


###  Vulples

- Deep belief network and connecting to the NVIDIA GPU 

---

###  Ariadne 

- Library for fitting Gaussian process regression models.

---

###  Numl  

- Perceptron
- K-Nearest Neighbors
- Decision Trees
- Naive Bayes 
- KMeans
- PCA
- Hierarchical Clusting

---

***




