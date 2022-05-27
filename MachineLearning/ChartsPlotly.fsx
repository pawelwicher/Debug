#r @"..\packages\FSharp.Plotly.1.1.21\lib\net47\FSharp.Plotly.dll"
open FSharp.Plotly

let x  = [1.; 2.; 3.; 4.; 5.; 6.; 7.; 8.; 9.; 10.; ]
let y  = [5.; 2.5; 5.; 7.5; 5.; 2.5; 7.5; 4.5; 5.5; 5.]
let y' = [2.; 1.5; 5.; 1.5; 3.; 2.5; 2.5; 1.5; 3.5; 1.]

[
    Chart.Point(x,y,Name="scattern");
    Chart.Line(x,y',Name="line")    
    |> Chart.withLineStyle(Width=2);
] 
|> Chart.Combine
|> Chart.Show

[ for x in 1.0 .. 100.0 -> (x, x ** 2.0) ]
|> Chart.Line |> Chart.Show