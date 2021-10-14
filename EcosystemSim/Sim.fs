namespace EcoSim

open System.Numerics
open Raylib_cs

type Blob =
    {
        Width: int
        Height: int
        Aggressiveness: int  // 1 to 10
    }
    
module private Utils =
    let CustomTriangle center distanceToPoint =
        0

module Sim =
    let passiveBlobList = [||]
    let aggroBlobList = [||]
    
    let blob = {Width = 20; Height = 20; Aggressiveness = 2}
    
    let Setup =
        Raylib.BeginDrawing ()
        Raylib.ClearBackground Color.BLACK
        let a =
            passiveBlobList
            |> Array.map (fun element -> Raylib.DrawTriangle (Vector2(1f, 2f), Vector2(1f, 2f), Vector2(1f, 2f), Color.RED))
        
        Raylib.EndDrawing ()
        

