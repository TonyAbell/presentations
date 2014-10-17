- title : Fsharp Data Science and R
- description : Introduction to Fsharp Data Science
- author : Tony Abell
- theme : Night
- transition : default

***

### What is Data Science ?

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
-Gives typed access to unstructured data

---

###  HTML Type Provider 
    type MarketDepth = HtmlProvider<"../data/MarketDepth.htm">
    let mrktDepth = MarketDepth.Load("http..").Tables.Table1
    let firstRow = mrktDepth.Rows |> Seq.head
    let settlementDate = firstRow.``Settlement Day``
    let acceptedBid = firstRow.``Accepted Bid Vol``
    let acceptedOffer = firstRow.``Accepted Offer Vol``

---

###  World Bank Type Provider
-The World Bank is an international organization that provides financial and technical assistance to developing countries around the world.
    
    let data = WorldBankData.GetDataContext()
    data
          .Countries.``United Kingdom``
          .Indicators.``School enrollment, tertiary (% gross)``
        |> Seq.maxBy fst

---

###  Freebase Type Provider
- The Freebase graph database contains information on over 23 million entities


    let data = FreebaseData.GetDataContext()
    let elements = data.``Science and Technology``.Chemistry.``Chemical Elements``
    let all = elements |> Seq.toList
    printfn "Elements found: %d" (Seq.length all)
    let hydrogen = elements.Individuals.Hydrogen
    printfn "Atominc number: %A" hydrogen.``Atomic number``


---

###  Cvs Type Provider
-foo

---

###  Hive Type Provider
- foo

---

###  SQL Type Provider
- foo

---

***

### Manipulation 
- Deedle
- IfSharp (iPython Notebook)

---

### Deedle
- FsLab

---

### IfSharp

- foo

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




