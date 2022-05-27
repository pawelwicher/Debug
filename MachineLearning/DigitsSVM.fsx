#r @"..\packages\Accord.3.8.0\lib\net462\Accord.dll"
#r @"..\packages\Accord.Math.3.8.0\lib\net462\Accord.Math.dll"
#r @"..\packages\Accord.Statistics.3.8.0\lib\net462\Accord.Statistics.dll"
#r @"..\packages\Accord.MachineLearning.3.8.0\lib\net462\Accord.MachineLearning.dll"
#r @"..\MachineLearning.BLL\bin\Debug\MachineLearning.BLL.dll"

open System
open System.IO
open System.Windows.Forms
open System.Drawing

open Accord.MachineLearning
open Accord.MachineLearning.VectorMachines
open Accord.MachineLearning.VectorMachines.Learning
open Accord.Statistics.Kernels

let root = __SOURCE_DIRECTORY__
let training = root + @"\data\digits\trainingsample.csv"
let validation = root + @"\data\digits\validationsample.csv"

let readData filePath =
    File.ReadAllLines filePath
    |> fun lines -> lines.[1..]
    |> Array.map (fun line -> line.Split(','))
    |> Array.map (fun line -> 
        (line.[0] |> Convert.ToInt32), (line.[1..] |> Array.map Convert.ToDouble))
    |> Array.unzip

let labels, observations = readData training

let drawDigit (pixels:float[], label:string) = 
        let tile = 20 
        let form = new Form(TopMost = true,  
                            Visible = true, 
                            Width = 29 * tile, 
                            Height = 29 * tile)
                  
        let panel = new Panel(Dock = DockStyle.Fill)
        panel.BackColor <- Color.Black
        form.Controls.Add(panel)
        let graphics = panel.CreateGraphics()  
        pixels
        |> Array.iteri (fun i p ->
            let col = i % 28
            let row = i / 28
            let color = Color.FromArgb(int p, int p, int p)
            let brush = new SolidBrush(color)
            graphics.FillRectangle(brush,col*tile,row*tile,tile,tile))
        let point = new PointF((float32)5, (float32)5)
        let font = new Font(family = FontFamily.GenericSansSerif, emSize = (float32)30)
        graphics.DrawString(label, font, new SolidBrush(Color.YellowGreen), point)
        form.Show()

//for i = 0 to 5 do
//    drawDigit(observations.[i], labels.[i].ToString())

let svm = MachineLearning.BLL.SVM.Learn(observations, labels)

let validationLabels, validationObservations = readData validation

let correct =
    Array.zip validationLabels validationObservations 
    |> Array.map (fun (l, o) -> if l = svm.Decide(o) then 1. else 0.)
    |> Array.average

let view =
    Array.zip validationLabels validationObservations 
    |> fun x -> x.[..20]
    |> Array.iter (fun (l, o) -> printfn "Real: %i, predicted: %i" l (svm.Decide(o)))

for i = 20 to 25 do
    drawDigit(validationObservations.[i], svm.Decide(validationObservations.[i]).ToString())
