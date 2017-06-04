module SudokuSolver
open System.Linq 
open System 
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
 

