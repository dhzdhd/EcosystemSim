namespace EcoSim

open System
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
    let getRandomFloat (start, stop) =
        let random = Random ()
        random.Next (start, stop)
            |> float32    

    let getRandomVector (start, stop, start', stop') =
        (getRandomFloat (start, stop), getRandomFloat (start', stop'))
            |> Vector2 

module Sim =
    let mutable blobList = List.empty

    module private Update =
        let wallCollision (blob: Blob) =
            match (blob.Center.X, blob.Center.Y) with
                |  _, _ -> () // return pos
        
        let moveBlob list =
            blobList <- list
                |> List.map (fun (element: Blob ) ->
                    let x = element.Center.X + element.Velocity.X
                    let y = element.Center.Y + element.Velocity.Y
                    wallCollision element
                    // update element with wall collision stuffs :)
                    {
                        element with
                            Center =  Vector2 (x, y)
                    })

    module private Draw =
        let initializeBlob () =
            let blob = {
                Center = Vector2(50.f, 50.f)
                Radius = 10.f
                Velocity = Utils.getRandomVector (1, 5, 1, 5)
                Color = Color.GREEN
                Type = BlobType.PassiveBlob
            }
            blobList <- blob :: blobList
            blobList <- {
                blob with
                    Velocity = Utils.getRandomVector (1, 5, 1, 5)
                    Center = Utils.getRandomVector (0, 1080, 0, 720)
            } :: blobList
            
        let createBlob blobType position=
            0
        
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
