namespace EcoSim

open System.Collections.Generic
open System.Numerics
open Raylib_cs

type BlobType =
    | PassiveBlob
    | AggroBlob
    | DiseasedBlob

type Blob =
    {
        Center: Vector2
        Radius: float32
        Color: Color
        Type: BlobType  // 1 to 10
    }

module private Utils =
    let CustomTriangle center distanceToPoint =
        0

module Sim =
//    let mutable diseasedBlobList = List.empty
    let mutable passiveBlobList = List.empty
//    let mutable aggroBlobList = List.empty
    
    let initializeBlob () =
        let blob = {Center = Vector2(50.f, 50.f); Radius = 10.f; Color = Color.GREEN; Type = BlobType.PassiveBlob}
        passiveBlobList <- blob :: passiveBlobList
    
    let drawBlobs () =
        passiveBlobList
            |> List.map (fun (element: Blob) ->
            Raylib.DrawCircle (int(element.Center.X) ,int(element.Center.Y), element.Radius, element.Color))
        
        
    let Setup () =
        // Update
        initializeBlob ()
        
        // Draw
        Raylib.BeginDrawing ()
        Raylib.ClearBackground Color.BLACK
        
//        drawBlobs ()
        Raylib.DrawCircle (int(passiveBlobList.[0].Center.X) ,int(passiveBlobList.[0].Center.Y), passiveBlobList.[0].Radius,passiveBlobList.[0].Color)
                
        Raylib.EndDrawing ()
        
        ()
        
        

