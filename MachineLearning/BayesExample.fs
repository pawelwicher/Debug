module BayesExample

open System
open Accord.MachineLearning
open Accord.Statistics.Distributions.Univariate
open Accord.MachineLearning.Bayes
open FSharp.Data

type DataProvider = CsvProvider<"data/train.csv">

type TrainDataRow = {
    sex : string
    age : float
    fare : decimal
    survived : bool
}

let getDataRows (path : string) =
    let data = DataProvider.Load(path)
    data.Rows |> Seq.filter (fun x -> Double.IsNaN(x.Age) = false) |> Seq.map (fun x -> { sex = x.Sex; age = x.Age; fare = x.Fare; survived = x.Survived }) |> Seq.toArray

let getInput (rows : TrainDataRow[]) =
    rows |> Array.map (fun (x : TrainDataRow) -> [|(if x.sex = "male" then 1.0 else 2.0); x.age; float x.fare|])

let getOutput (rows : TrainDataRow[]) =
    rows |> Array.map (fun (x : TrainDataRow) -> (if x.survived then 1 else 0))

let run () =
    let trainDataRows = getDataRows("data/train.csv")
    let input = getInput trainDataRows
    let output = getOutput trainDataRows
    let bayes = NaiveBayesLearning<NormalDistribution>()

    let nb = bayes.Learn(input, output)

    let answers  = nb.Decide(input) |> Array.map (fun x -> x = 1)

    let mutable count = 0

    for i = 0  to trainDataRows.Length - 1 do
        if trainDataRows.[i].survived = answers.[i] then
            count <- count + 1
        printfn "%A" trainDataRows.[i]
        printfn "Decision: %b" answers.[i]
        printfn ""
     
    printfn "%i / %i" count trainDataRows.Length