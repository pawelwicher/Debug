#r @"..\packages\Google.DataTable.Net.Wrapper.3.1.2.0\lib\Google.DataTable.Net.Wrapper.dll"
#r @"..\packages\XPlot.GoogleCharts.1.5.0\lib\net45\XPlot.GoogleCharts.dll"
#r @"..\packages\XPlot.Plotly.1.5.0\lib\net45\XPlot.Plotly.dll"
open XPlot.GoogleCharts

let Bolivia = ["2004/05", 165.; "2005/06", 135.; "2006/07", 157.; "2007/08", 139.; "2008/09", 136.]
let Ecuador = ["2004/05", 938.; "2005/06", 1120.; "2006/07", 1167.; "2007/08", 1110.; "2008/09", 691.]
let Madagascar = ["2004/05", 522.; "2005/06", 599.; "2006/07", 587.; "2007/08", 615.; "2008/09", 629.]
let Average = ["2004/05", 614.6; "2005/06", 682.; "2006/07", 623.; "2007/08", 609.4; "2008/09", 569.6]

let series = [ "bars"; "bars"; "bars"; "lines" ]
let inputs = [ Bolivia; Ecuador; Madagascar; Average ]

inputs
|> Chart.Combo
|> Chart.WithOptions 
     (Options(title = "Coffee Production", 
              series = [| for typ in series -> Series(typ) |]))
|> Chart.WithLabels ["Bolivia"; "Ecuador"; "Madagascar"; "Average"]
|> Chart.WithLegend true
|> Chart.WithSize (600, 250)
|> Chart.Show

Chart.Line [ for x in 0. .. 0.5 .. 6.3 -> x, sin x ]
|> Chart.Show

Chart.Line [ for x in 0. .. 0.5 .. 6.3 -> x, sin x ]
|> Chart.WithOptions(Options(curveType = "function"))
|> Chart.Show

let rnd = new System.Random() 
let next() = rnd.NextDouble() * rnd.NextDouble()
let points1 = [ for i in 0 .. 100 -> next(), next() ]
let points2 = [ for i in 0 .. 100 -> next(), next() ]
let points3 = [ for i in 0 .. 100 -> next(), next() ]
[points1; points2; points3] |> Chart.Scatter |> Chart.WithLabels ["1"; "2"; "3"] |> Chart.WithSize (500, 500) |> Chart.WithTitle "Random points" |> Chart.Show

["PL", 1] |> Chart.Geo |> Chart.Show 



open XPlot.Plotly

let layout = Layout(title = "Basic Bar Chart")

let data = ["giraffes", 20; "orangutans", 14; "monkeys", 23]

data
|> Chart.Bar
|> Chart.WithLayout layout
|> Chart.WithHeight 500
|> Chart.WithWidth 700
|> Chart.Show