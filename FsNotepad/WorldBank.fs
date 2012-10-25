module WorldBank

open System.Web
open System.Xml.Linq

open WebHelpers

let queryParms(props) =
    seq { for key, value in props do
              yield key + "=" + HttpUtility.UrlEncode(value:string) }
    |> String.concat "&" |> (+) "&"

let worldBankUrl(functions, props) =
    seq { 
        yield "http://api.worldbank.org"
        for item in functions do
            yield "/" + HttpUtility.UrlEncode(item:string)
        yield "?per_page=100"
        yield queryParms(props) }
//        for key, value in props do
//            yield "&" + key + "=" + HttpUtility.UrlEncode(value:string) }
    |> String.concat ""

let worldBankDownload(properties) =
    let url = worldBankUrl(properties)
    let rec loop(attempts) = async {
        try
            return! downloadUrl(url)
        with _ when attempts > 0 ->
            printfn "Failed, retrying (%d): %A" attempts properties
            do! Async.Sleep(500)
            return! loop(attempts - 1) }
    loop(20)

let worldBankRequest(props) = async {
    let! text = worldBankDownload(props)
    return XDocument.Parse(text) }

