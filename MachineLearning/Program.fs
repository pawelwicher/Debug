open System

[<EntryPoint>]
let main _ =
    KMeansExample.run()
    //BayesExample.run()
    printfn "Press any key..."
    Console.ReadKey() |> ignore
    0