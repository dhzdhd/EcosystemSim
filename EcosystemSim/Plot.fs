namespace EcoSim

open Plotly.NET

module Plot =
    let xData1 = [0. .. 10.]
    let yData1 = [0. .. 10.]
    
    let xData2 = [10. .. 20.]
    let yData2 = [10. .. 20.]
    
    let ShowData =
        let chart =
            [
                Chart.Line(xData1, yData1)
                    |> Chart.withTitle "Hello World!"
                    |> Chart.withXAxisStyle ("X ->", ShowGrid = false)
                    |> Chart.withYAxisStyle ("Y ->", ShowGrid = false)
                Chart.Line(xData2, yData2)
                    |> Chart.withTitle "Hello World!"
                    |> Chart.withXAxisStyle ("X ->", ShowGrid = false)
                    |> Chart.withYAxisStyle ("Y ->", ShowGrid = false)
            ]
            |> Chart.combine
            |> Chart.show
            
        chart     
       
        ()
