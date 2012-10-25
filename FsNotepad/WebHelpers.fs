module WebHelpers

open Microsoft.FSharp.Control.WebExtensions

let downloadUrl(url:string) = async {
    let request = System.Net.HttpWebRequest.Create(url, Proxy = System.Net.WebProxy("http://127.0.0.1:3128"))
    use! response = request.AsyncGetResponse()
    let stream = response.GetResponseStream()
    use reader = new System.IO.StreamReader(stream)
    return! reader.AsyncReadToEnd() }

