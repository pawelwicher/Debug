#r @"..\packages\MathNet.Numerics.3.5.0\lib\net40\MathNet.Numerics.dll"
#r @"..\packages\MathNet.Numerics.FSharp.3.5.0\lib\net40\MathNet.Numerics.FSharp.dll"

open MathNet.Numerics.LinearAlgebra

let a = matrix [[ 5.; 6.; 1. ]
                [ 8.; 7.; 9. ]
                [ 1.; 5.; 2. ]]

let b = matrix [[ 4.; 6.; 7. ]
                [ 2.; 5.; 1. ]
                [ 0.; 3.; 9. ]]

let c = a * b

printfn "%A" c