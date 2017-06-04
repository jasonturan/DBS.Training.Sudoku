module SudokuTests
open Microsoft.VisualStudio.TestTools.UnitTesting
open System.Linq 
open System
open FSharp.Collections.ParallelSeq
open DBS.Training.Sudoku.SudokuSolver
 

[<TestClass>]
type TestSudoku() =    
    member this.UnsolvedPuzzle =
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
            
    member this.SolvedPuzzle =  
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

    member this.UnsolvablePuzzle =  
        let unsolvableArrays = [| 
                [| 1;2;3;4;5;6;7;8;0 |];
                [| 0;0;0;0;0;0;0;0;2 |];
                [| 0;0;0;0;0;0;0;0;3 |];
                [| 0;0;0;0;0;0;0;0;4 |];
                [| 0;0;0;0;0;0;0;0;5 |];
                [| 0;0;0;0;0;0;0;0;6 |];
                [| 0;0;0;0;0;0;0;0;7 |];
                [| 0;0;0;0;0;0;0;0;8 |];
                [| 0;0;0;0;0;0;0;0;9 |];
           |]
        Array2D.init columnCount rowCount (fun i j -> unsolvableArrays.[i].[j]) 

         
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
            )                

     [<TestMethod>]
     member this.ShouldReturnNoneForUnsolvablePuzzle() =
        let p = this.UnsolvablePuzzle
        let solved = Solve(p)
        Assert.IsTrue(solved.IsNone)

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
        let nextToSolve = GetNextUnsolved this.UnsolvedPuzzle 
        Assert.AreEqual(Some((8,7)), nextToSolve)

        let solved = GetNextUnsolved this.SolvedPuzzle 
        Assert.AreEqual(None, solved)            

    [<TestMethod>]
    member this.ShouldEvaluatePuzzle() =
        let newPuzzle = GetFirstPuzzle()
        Assert.IsFalse(HasBeenSolved newPuzzle)         
        Assert.IsFalse(HasBeenSolved this.UnsolvedPuzzle) 
        Assert.IsTrue(HasBeenSolved this.SolvedPuzzle)