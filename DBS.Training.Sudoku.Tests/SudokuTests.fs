module SudokuTests
open Microsoft.VisualStudio.TestTools.UnitTesting
open System.Linq

let inline add x y = x + y

[<TestClass>]
type TestSudoku() = 
    
    [<TestMethod>]
    member this.ShouldReadFile() =  
         let line = System.IO.File.ReadLines("sudoku.txt").First()
         let columnCount =9 
         let rowCount = 9

         let a = Array2D.init columnCount rowCount (fun x y -> line.Substring(x + (columnCount*y) ,1)|> int)
         printfn "%A" a
         //printfn "%s" line

 
//
//namespace DBS.Training.Sudoku.Tests 
////https://wiki.haskell.org/Sudoku
//open NUnit.Framework
//open FsUnit
//let inline add x y = x + y
//
//[<Test>]
//let ``When 2 is added to 2 expect 4``() = 
//    add 2 2 |> should equal 4
//
//[<Test>]
//let ``When 2.0 is added to 2.0 expect 4.01``() = 
//    add 2.0 2.0 |> should (equalWithin 0.1) 4.01
//
//[<Test>]
//let ``When ToLower(), expect lowercase letters``() = 
//    "FSHARP".ToLower() |> should startWith "fs"