#nowarn "211"
// Try including various folders where Deedle might be (version updated by FAKE)

#I "packages/Deedle/lib/net40"

// Also reference path with FSharp.Data.DesignTime.dll
#I "packages/FSharp.Data/lib/net40/"
// Reference Deedle
#r "Deedle.dll"

do fsi.AddPrinter(fun (printer:Deedle.Internal.IFsiFormattable) -> "\n" + (printer.Format()))
open Deedle
