
#nowarn "211"
// Try including various folders where RProvider might be (version updated by FAKE)

#I @"packages\R.NET.Community\lib\net40"
#I @"packages\RProvider\lib\net40"
// Reference RProvider and RDotNet 
#r "RDotNet.dll"
#r "RProvider.dll"
#r "RProvider.Runtime.dll"
open RProvider

do fsi.AddPrinter(fun (synexpr:RDotNet.SymbolicExpression) -> synexpr.Print())

