namespace EcoSim

open System
open System.Collections.Generic
open System.Numerics
open Microsoft.VisualBasic
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
    
type Food =
    {
        Center: Vector2
        Radius: float32
        Lifetime: int
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
            if
                blob.Center.X + blob.Radius > float32(Constants.WIDTH)
                || blob.Center.X - blob.Radius < 0.f
            then Vector2 (blob.Velocity.X * -1.f, blob.Velocity.Y)
            elif
                blob.Center.Y + blob.Radius > float32(Constants.HEIGHT)
                || blob.Center.Y - blob.Radius < 0.f
            then Vector2 (blob.Velocity.X, blob.Velocity.Y * -1.f)
            else Vector2 ()
                
        let moveBlob list =
            blobList <- list
                |> List.map (fun (element: Blob) ->
                    let result = wallCollision element
                    
                    let vX, vY =
                        if result <> Vector2 () then result.X, result.Y else element.Velocity.X, element.Velocity.Y
                    
                    let x = element.Center.X + vX
                    let y = element.Center.Y + vY
                        
                    {
                        element with
                            Center = Vector2 (x, y)
                            Velocity = Vector2 (vX, vY)
                    })

    module private Draw =
        let initializeBlob () =
            let blob = {
                Center = Vector2(50.f, 50.f)
                Radius = 10.f
                Velocity = Utils.getRandomVector (-5, 5, -5, 5)
                Color = Color.GREEN
                Type = BlobType.PassiveBlob
            }
            blobList <- blob :: blobList
            blobList <- {
                blob with
                    Velocity = Utils.getRandomVector (-5, 5, -5, 5)
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
