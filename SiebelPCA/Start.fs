open System
open System.IO
open FSharp.Configuration
open canopy.classic
open canopy.runner.classic

type Settings = AppSettings<"App.config">

start chrome

let openPage () =
    let login, password = Settings.Login, Settings.Password
    let pageUrl = sprintf "http://%s:%s@siebel.delaval.local/service_enu" login password
    url pageUrl
    printfn "open page"
    sleep 5

let selectPCAsTab () =
    let id = sprintf "#s_sctrl_tabScreen > ul > li:nth-child(%i) > a" Settings.PcaTabIndex
    let mutable stop = false 
    while not stop do
        sleep 5
        if (someElement id).IsSome then
            stop <- true
    click id
    printfn "select pca's tab"
    sleep 5

let selectAllPCAsInDropdown () =
    click "#s_vis_div > select"
    click (sprintf "#s_vis_div > select > option:nth-child(%i)" Settings.AllPcaSIndex)
    printfn "select all pca's in dropdown"
    sleep 3

let selectQueryInDropdown () =
    click "#PDQToolbar > div.PageItem > select"
    click (sprintf "#PDQToolbar > div.PageItem > select > option:nth-child(%i)" Settings.QueryIndex)
    printfn "select query in dropdown"
    sleep 3

let selectFirstItemOnThePCAsList () =
    click "#s_1_l > tbody > tr:nth-child(2) > td:nth-child(2) > a"
    printfn "select first pca"
    sleep 3

let selectPCAsHistoryTab () =
    click (sprintf "#s_vctrl_div_tabScreen > ul > li:nth-child(%i) > a" Settings.PcaHistoryTabIndex)
    printfn "select history tab"
    sleep 3

let exportCurrentPCA () =
    let export () =
        click "#s_at_m_1"
        click "#s_at_m_1-menu > li:last > a"
        sleep 1
        click "form[name='SIPopupIEForm'] > table > tbody > tr:nth-child(2) > td > table > tbody > tr > td > span:nth-of-type(2) > span > a"
        sleep 3
        click "form[name='SIPopupIEForm'] > table > tbody > tr:nth-child(2) > td > table > tbody > tr > td > span:nth-of-type(3) > span > a"
        sleep 2
    export()
    let fileExists = File.Exists(Settings.DownloadsPath + "output.csv")
    if not fileExists then
        export()

let getCurrentId () =
    read "[name='s_2_1_31_0']"

let clickNextPCA () =
    click "#s_2_1_97_0 > a"

let moveFile (newName : string) =
    Directory.Move(Settings.DownloadsPath + "output.csv", Settings.WorkingPath + newName + ".csv")

let appendHistoryToFile (pcaId : string) =
    let lines = File.ReadAllLines(Settings.WorkingPath + pcaId + ".csv") |> Array.tail |> Array.map(fun (line : string) -> ("\"" + pcaId + "\"") + "\t" + line)
    File.AppendAllLines(Settings.WorkingPath + "pcaHistory.csv", lines)

let exportPCAsHistory () =
    let mutable pcaId = ""
    let mutable stop = false
    let mutable i = 0
    printfn "export pca history"
    while not stop do
        let id = getCurrentId()
        if id = pcaId then
            stop <- true
        else
            pcaId <- id
            i <- i + 1
            exportCurrentPCA()
            moveFile pcaId
            appendHistoryToFile pcaId
            printfn "%i %s" i pcaId
            clickNextPCA()

"SiebelPCA" &&& fun _ ->
    openPage()
    selectPCAsTab()
    selectAllPCAsInDropdown()
    selectQueryInDropdown()
    selectFirstItemOnThePCAsList()
    selectPCAsHistoryTab()
    exportPCAsHistory()

run()

"Press Enter to exit." |> Console.WriteLine
Console.ReadLine() |> ignore
quit()