#r "System.Xml.dll"
#r "System.Xml.Linq.dll"
#r "..\lib\FSharpPowerPack\FSharp.PowerPack.dll"
#r "..\lib\Office\Microsoft.Office.Interop.Excel.dll"

#load "WebHelpers.fs"
#load "WorldBank.fs"
open WorldBank

#load "XmlHelpers.fs"
open XmlHelpers

let doc =
    worldBankRequest(["countries"], ["region", "ECS"])
    |> Async.RunSynchronously

let regions = seq { 
    let countries = doc |> xnested [ "countries" ]
    for country in countries |> xelems "country" do
        yield country |> xelem "name" |> xvalue }


let rec getIndicatorData(date, indicator, page) = async {
    let args = [ "countries"; "all"; "indicators"; indicator ],
               [ "date", date; "page", string(page)]
    let! doc = worldBankRequest args

    let pages =
        doc |> xnested [ "data" ]
            |> xattr "pages" |> int

    if (pages = page) then
        return [doc]
    else
        let! rest = getIndicatorData(date, indicator, page + 1)
        return doc::rest }

let downloadAll = seq {
    for ind in [ "AG.SRF.TOTL.K2"; "AG.LND.FRST.ZS" ] do
        for year in [ "1990:1990"; "2000:2000"; "2005:2005" ] do
            yield getIndicatorData(year, ind, 1) }

let data = Async.RunSynchronously(Async.Parallel(downloadAll))

let readSingleValue parse node =
    let value = node |> xelem "value" |> xvalue
    let country = node |> xelem "country" |> xvalue
    let year = node |> xelem "date" |> xvalue |> int
    if (value = "") then []
    else [ (year, country), parse(value) ]

let readValues parse data = seq {
    for page in data do
        let root = page |> xnested [ "data" ]
        for node in root |> xelems "data" do
        yield! node |> readSingleValue parse }

data.[0] |> readValues id 

#load "Units.fs"
open Measures

let areas =
    Seq.concat(data.[0..2])
        |> readValues (fun a -> float(a) * 1.0<km^2>)
        |> Map.ofSeq

let forests =
    Seq.concat(data.[3..5])
        |> readValues (fun a -> float(a) * 1.0<percent>)
        |> Map.ofSeq

let calculateForests(area:float<km^2>, forest:float<percent>) =
    let forestArea = forest * area
    forestArea / 100.0<percent>


let years = [ 1990; 2000; 2005 ]
let dataAvailable(key) =
    years |> Seq.forall (fun y ->
        (Map.containsKey (y, key) areas) &&
        (Map.containsKey (y, key) forests));;

let getForestData(key) =
    [| for y in years do
        yield calculateForests(areas.[y, key], forests.[y, key]) |];;

let stats = seq {
    for name in regions do
        if dataAvailable(name) then
            yield name, getForestData(name) };;


#load "Report.fs"
open Report

Report.print stats

