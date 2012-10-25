module Report

open System
open Microsoft.Office.Interop.Excel
open Measures

let print (stats: seq<string * float<km ^ 2> []>) =
    let app = new ApplicationClass(Visible = true)
    let workbook = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet)
    let worksheet = (workbook.Worksheets.[1] :?> _Worksheet)
    worksheet.Range("C2").Value2 <- "1990"
    worksheet.Range("C2", "E2").Value2 <- [| "1990"; "2000"; "2005" |]

    let statsArray = stats |> Array.ofSeq
    let names = Array2D.init statsArray.Length 1 (fun index _ ->
        let name, _ = statsArray.[index]
        name )

    let dataArray = Array2D.init statsArray.Length 3 (fun index year ->
        let _, values = statsArray.[index]
        let yearValue = values.[year]
        yearValue / 1000000.0 )

    let endColumn = string(statsArray.Length + 2)

    worksheet.Range("B3", "B" + endColumn).Value2 <- names
    worksheet.Range("C3", "E" + endColumn).Value2 <- dataArray

    let chartobjects = (worksheet.ChartObjects() :?> ChartObjects)
    let chartobject = chartobjects.Add(400.0, 20.0, 550.0, 350.0)
    chartobject.Chart.ChartWizard(
        Title = "Area covered by forests",
        Source = worksheet.Range("B2", "E" + endColumn),
        Gallery = XlChartType.xl3DColumn,
        PlotBy = XlRowCol.xlColumns,
        SeriesLabels = 1, CategoryLabels = 1,
        CategoryTitle = "", 
        ValueTitle = "Forests (mil km^2)")

    // Office 2007 or later
    //chartobject.Chart.ChartType <- 5 

