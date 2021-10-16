namespace EcoSim

open Raylib_cs

module Program =
    let mutable start = false
    let mutable pause = false
    
    
    [<EntryPoint>]
    let main argv =
        Raylib.InitWindow (1080, 720, "Ecosystem Simulation")
        Raylib.SetConfigFlags ConfigFlags.FLAG_WINDOW_RESIZABLE
        Raylib.SetTargetFPS 60
        
        while (Raylib.WindowShouldClose() <> true) do
            StartScreen.Setup ()
            Sim.Setup ()
            
        
        Raylib.CloseWindow ()
        
        Plot.ShowData Sim.blobList Sim.blobList Sim.blobList
        
        0
