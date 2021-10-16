namespace EcoSim

open System
open Plotly.NET
open Plotly.NET.ImageExport

module Plot =
    let x = [1. .. 10.]
    let y =
        x
        |> List.map (fun ele -> ele |> Math.Log |> Math.Sin)
    
    let chartDesc =
        ChartDescription.create "hello"
            <| """<h1 class="title" style="color: rgb(255, 0, 0)">Heading</h1>"""
    
    let ShowData blobList =
        let chart =
            [
                Chart.Spline(x, y)
                Chart.Spline(x, y)
                Chart.Spline(x, y)
            ]
            |> Chart.combine
            |> Chart.withTitle "Ecosystem simulation!"
            |> Chart.withDescription chartDesc
            |> Chart.withXAxisStyle ("Time", ShowGrid = false)
            |> Chart.withYAxisStyle ("Population", ShowGrid = false)
        
        chart |> Chart.show
        chart |> Chart.savePNG "../../../../chart"
        
        printfn "Saved chart image successfully!"
       
        ()
