#r @"..\packages\FSharp.Charting.0.90.9\lib\net40\FSharp.Charting.dll"

open FSharp.Charting
open FSharp.Charting.ChartTypes
open System.Windows.Forms

let chart = Chart.Line [ for x in 1.0 .. 100.0 -> (x, x ** 2.0) ]
let chartControl = new ChartControl(chart, Dock=DockStyle.Fill)
let form = new Form(Visible = true, TopMost = true, Width = 700, Height = 500)
form.Controls.Add(chartControl)
do Application.Run(form) |> ignore