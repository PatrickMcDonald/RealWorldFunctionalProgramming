module XmlHelpers

open System.Xml.Linq

let wb = "http://www.worldbank.org"

let xattr s (el:XElement) =
    el.Attribute(XName.Get(s)).Value

let xelem s (el:XContainer) =
    el.Element(XName.Get(s, wb))

let xvalue (el:XElement) =
    el.Value

let xelems s (el:XContainer) =
    el.Elements(XName.Get(s, wb))

let xnested path (el:XContainer) =
    let res = path |> Seq.fold (fun xn s ->
        let child = xelem s xn
        child :> XContainer) el
    res :?> XElement

