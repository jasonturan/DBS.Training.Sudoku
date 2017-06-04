module SudokuTests
open Microsoft.VisualStudio.TestTools.UnitTesting
open System.Linq 
open System
open FSharp.Collections.ParallelSeq
let columnCount =9 
let rowCount = 9

let ConstructPuzzle(line : string) =
    Array2D.init columnCount rowCount (fun x y -> line.Substring(x + (columnCount*y) ,1)|> int)       

let GetFirstPuzzle() =  
        let line = System.IO.File.ReadLines("sudoku.txt").First()        
        ConstructPuzzle(line)

let ConstructNewPuzzle(s:int[,], row:int, column:int, newVal:int)=
    Array2D.init columnCount rowCount (fun x y -> 
        if(x = row && y = column) 
        then newVal 
        else  Array2D.get s x y)

let flatten (A: 'a[,])= A |> Seq.cast<'a>
let getColumn c (A:_[,])=
    flatten A.[*,c..c] |> Seq.toArray
let getRow r (A: _[,])=
    flatten A.[r..r,*] |> Seq.toArray


let GetChoices (s:int[,], row:int, column:int): Set<int> =  
      let column:int[] = getColumn column s
      let row :int[] = getRow row s
      let allChoices = Set.ofSeq (seq {for n in 1..9 do yield n})
      let takenChoices = Set.ofArray(Array.distinct (Array.append column row))
      allChoices - takenChoices 

let HasBeenSolved (s:int[,]) :bool =
    let required = Set.ofSeq (seq {for n in 1..9 do yield n})
    seq{for n in 0..8 do 
        yield Set.ofArray(getRow (n) s) = required && 
              Set.ofArray(getColumn (n) s) = required
       }
       |>Seq.forall (fun pass-> pass)

let GetNextUnsolved (s:int[,]) : Option<int*int>  =
    let points = 
        [0..8] |> 
            Seq.collect(fun row -> Seq.map (fun col -> (row,col)) [0..8])            
    Seq.tryFind(fun point -> 
        let (x,y) = point
        s.[x,y] = 0) points 

let rec Solve (p:int[,]) : Option<int[,]> =
    if HasBeenSolved p then Some(p)
    else 
        let x=GetNextUnsolved(p) 
        match x with 
            |Some (x,y) -> 
                let choices = GetChoices(p, x,y) 
                let sp = Seq.fold (fun (acc :Option<int[,]>) (newVal:int) -> 
                    if(acc.IsSome) then acc 
                    else
                        let newPuzzle = ConstructNewPuzzle(p,x,y, newVal) 
                        Solve(newPuzzle)) None choices
                sp          
            |None -> failwith "Should have found the next unsolved"    
 
[<TestClass>]
type TestSudoku() =    
    member this.CreateUnsolvedPuzzle =
        let unsolvedArrays = [| 
                [| 8;2;7;1;5;4;3;9;6 |];
                [| 9;6;5;3;2;7;1;4;8 |];
                [| 3;4;1;6;8;9;7;5;2 |];
                [| 5;9;3;4;6;8;2;7;1 |];
                [| 4;7;2;5;1;3;6;8;9 |];
                [| 6;1;8;9;7;2;4;3;5 |];
                [| 7;8;6;2;3;5;9;1;4 |];
                [| 1;5;4;7;9;6;8;2;3 |];
                [| 2;3;9;8;4;1;5;0;7 |];
           |]
        Array2D.init columnCount rowCount (fun i j -> unsolvedArrays.[i].[j])
            
    member this.CreateSolvedPuzzle =  
        let solvedArrays = [| 
                [| 8;2;7;1;5;4;3;9;6 |];
                [| 9;6;5;3;2;7;1;4;8 |];
                [| 3;4;1;6;8;9;7;5;2 |];
                [| 5;9;3;4;6;8;2;7;1 |];
                [| 4;7;2;5;1;3;6;8;9 |];
                [| 6;1;8;9;7;2;4;3;5 |];
                [| 7;8;6;2;3;5;9;1;4 |];
                [| 1;5;4;7;9;6;8;2;3 |];
                [| 2;3;9;8;4;1;5;6;7 |];
           |]
        Array2D.init columnCount rowCount (fun i j -> solvedArrays.[i].[j]) 

         
    [<TestMethod>]
    member this.ShouldReadFile() =  
        let x = GetFirstPuzzle()
        printfn "%A" x
        
     [<TestMethod>]
     member this.ShouldDetermineAvailableChoices() =  
        let x = GetFirstPuzzle()
        let choices = GetChoices (x, 0, 0 )
        Assert.AreEqual (Set.ofArray([|2;5;6;7;8;9|]),choices)
     
     [<TestMethod>]
     member this.ShouldConstructNewPuzzle() =
        let p = GetFirstPuzzle()
        let choices = GetChoices(p,0,0)
        let newPuzzle =ConstructNewPuzzle(p, 0,0,choices.First())
        Assert.AreEqual(choices.First(),newPuzzle.GetValue(0,0))

     [<TestMethod>]
     member this.ShouldSolveAllPuzzles() =
        System.IO.File.ReadLines("sudoku.txt") |>
            Seq.take 10 |>
            PSeq.iter (fun line ->
                let p =ConstructPuzzle(line)
                let maybeSolved = Solve(p)
                let nl = System.Environment.NewLine
                match maybeSolved with
                    |Some(solved)-> printfn "Input%s%A%sSolved%s%A" nl p nl nl solved
                    |None -> ()
                //printfn "test"
            )                


     [<TestMethod>]
     member this.ShouldSolvePuzzle() =
        let p = GetFirstPuzzle()
        printfn "Puzzle"
        printfn "%A" p

        let solved = Solve(p)
        printfn "Solved"
        printfn "%A" solved.Value 

    [<TestMethod>]
    member this.ShouldGetNextUnsolved() = 
        let nextToSolve = GetNextUnsolved this.CreateUnsolvedPuzzle 
        Assert.AreEqual(Some((8,7)), nextToSolve)

        let solved = GetNextUnsolved this.CreateSolvedPuzzle 
        Assert.AreEqual(None, solved)            

    [<TestMethod>]
    member this.ShouldEvaluatePuzzle() =
        let newPuzzle = GetFirstPuzzle()
        Assert.IsFalse(HasBeenSolved newPuzzle)         
        Assert.IsFalse(HasBeenSolved this.CreateUnsolvedPuzzle) 
        Assert.IsTrue(HasBeenSolved this.CreateSolvedPuzzle)

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
//    "FSHARP".ToLower() |> should s9tartWith "fs"