module KMeansExample

open System
open Accord.MachineLearning
open FSharp.Charting

let run () =
    let rnd = new Random()
    let getRandomPoint () =
        let k = rnd.Next(3)
        (rnd.Next(100) + k * 200), (rnd.Next(100) + k * 200)
    let toChartData data filter = data |> Array.filter (fun (i, _) -> i = filter) |> Array.map (fun (_, x) -> x)
    let toModelData data = data |> Seq.toArray |> Array.map (fun (x,y) -> [|float x; float y|])

    let data = [for _ in 1 .. 100 -> getRandomPoint()]
    let observations = toModelData data
    let kmeans = new KMeans(3)
    let clusters = kmeans.Learn(observations)
    let decisions = clusters.Decide(observations)
    let output = decisions |> Array.mapi (fun i x -> x, data.[i])
    let g0 = toChartData output 0
    let g1 = toChartData output 1
    let g2 = toChartData output 2

    let testData = [(380, 350); (120, 120); (180, 180); (380, 380)]
    let testObservations = toModelData testData
    let testDecisions = clusters.Decide(testObservations)
    let testOutput = testDecisions |> Array.mapi (fun i x -> x, testData.[i])
    let h0 = toChartData testOutput 0
    let h1 = toChartData testOutput 1
    let h2 = toChartData testOutput 2

    let f = 
        (Chart.Combine [
            g0 |> Chart.Point |> Chart.WithDataPointLabels("0")
            g1 |> Chart.Point |> Chart.WithDataPointLabels("1")
            g2 |> Chart.Point |> Chart.WithDataPointLabels("2")
            h0 |> Chart.Point |> Chart.WithDataPointLabels("0")
            h1 |> Chart.Point |> Chart.WithDataPointLabels("1")
            h2 |> Chart.Point |> Chart.WithDataPointLabels("2")
        ]).ShowChart()
    System.Windows.Forms.Application.Run(f)
