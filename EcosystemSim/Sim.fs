namespace EcoSim

open System
open System.Collections.Generic
open System.Numerics
open Microsoft.VisualBasic.CompilerServices
open Raylib_cs

type BlobType =
    | PassiveBlob
    | AggroBlob
    | DiseasedBlob

type Blob =
    {
        Center: Vector2
        Radius: float32
        Velocity: Vector2
        Color: Color
        Type: BlobType
    }
    
type BlobCount =
    {
        blob: Blob[]
        count: int
    }

module private Utils =
    let getRandomVector (start, stop) =
        let random = Random ()
        random.Next (start, stop)
            |> float32
        

module Sim =
    let mutable blobList = List.empty

    module private Update =
        let moveBlob list =
            blobList <- list
                |> List.map (fun (element: Blob ) ->
                    {
                        element with
                            Center = Vector2 (Utils.getRandomVector (1, 10) , Utils.getRandomVector (1, 10))
                    })

    module private Draw =
        let initializeBlob () =
            let blob = {
                Center = Vector2(50.f, 50.f)
                Radius = 10.f
                Velocity = Vector2 (Utils.getRandomVector (1, 10), Utils.getRandomVector (1, 10))
                Color = Color.GREEN
                Type = BlobType.PassiveBlob
            }
            blobList <- {
                blob with
                    Velocity = Vector2 (Utils.getRandomVector (1, 10), Utils.getRandomVector (1, 10))
                    Center = Vector2 (Utils.getRandomVector (0, 1080), Utils.getRandomVector (0, 720))
            } :: blobList
        
        let drawBlobs () =
            [blobList]
                |> List.map (fun list ->
                    list |> List.map (fun element -> 
                        Raylib.DrawCircle (int(element.Center.X) ,int(element.Center.Y), element.Radius, element.Color)))
        
    Draw.initializeBlob ()  
      
    let Setup () =
        // Update
        Update.moveBlob blobList
        
        // Draw
        Raylib.BeginDrawing ()
        Raylib.ClearBackground Color.BLACK
        
        Draw.drawBlobs ()
                
        Raylib.EndDrawing ()
        
        ()
