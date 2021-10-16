namespace EcoSim

open Raylib_cs

module StartScreen =
    let Setup () =
        // Update
        if Raylib.IsKeyPressed KeyboardKey.KEY_ENTER then printfn ""
        
        // Draw
        Raylib.BeginDrawing ()
        Raylib.ClearBackground Color.BLACK
        
        Raylib.DrawText ("Press Enter to Start!", 50, 50, 30, Color.GREEN)
        
        Raylib.EndDrawing ()