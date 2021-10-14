namespace EcoSim

open Raylib_cs
open Sim

module Program =
    let mutable start = false
    let mutable pause = false
    
    
    [<EntryPoint>]
    let main argv =
        Raylib.InitWindow (1080, 720, "Ecosystem Simulation")
        Raylib.SetConfigFlags ConfigFlags.FLAG_WINDOW_RESIZABLE
        
        while (Raylib.WindowShouldClose () <> true) do
            Setup
        
        Raylib.CloseWindow ()
        0