namespace EcoSim

open Raylib_cs

module Program =
    let mutable start = true
    
    [<EntryPoint>]
    let main argv =
        Raylib.InitWindow (Constants.WIDTH, Constants.HEIGHT, "Ecosystem Simulation")
        Raylib.SetConfigFlags ConfigFlags.FLAG_WINDOW_RESIZABLE
        Raylib.SetTargetFPS 60
        
        while (Raylib.WindowShouldClose() <> true) do
            match start with 
               | false -> StartScreen.Setup ()
               | true -> Sim.Setup ()
            
        
        Raylib.CloseWindow ()
        
//        Plot.ShowData Sim.blobList 
        
        0
