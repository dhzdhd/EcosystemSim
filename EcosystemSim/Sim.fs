namespace EcoSim

open System
open System.Collections.Generic
open System.Drawing
open System.Numerics
open System.Reflection.Metadata
open System.Threading
open System.Timers
open EcoSim
open Microsoft.VisualBasic
open Microsoft.VisualBasic.CompilerServices
open Plotly.NET
open Plotly.NET
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
        Lifetime: float32
        Ticker: int
    }
    
type Food =
    {
        Center: Vector2
        Radius: float32
        Color: Color
        Lifetime: float32
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
    let mutable foodList = List.empty
    let mutable secondTicker = 0.
    let mutable ticker = 0
    let mutable paused = true

    module private Update =
        let updateTickerCounter () =
            secondTicker <- secondTicker + (1. / 60.)
            ticker <- ticker + 1
            
            blobList <- blobList
            |> List.map (fun element ->
                {element with Ticker = element.Ticker + 1})
            
        let onCollision () =
            // Blob and food
            blobList
            |> List.iter (fun (element: Blob) ->
                foodList <- foodList |> List.filter (fun (element': Food) ->
                    let collision = Raylib.CheckCollisionCircles (element.Center, element.Radius, element'.Center, element'.Radius)
                    
                    if collision then
                        blobList <- blobList
                            |> List.filter (fun blob -> blob <> element)
                            |> List.append [{element with Lifetime = 100.f}]
                    
                    not collision
                    )
                )
            
            // Blob and blob
//            blobList
//            |> List.iter (fun (element: Blob) ->
//                blobList
//                    |> List.iter (fun (element': Blob) ->
//                        let collision = Raylib.CheckCollisionCircles (element.Center, element.Radius, element'.Center, element'.Radius)
//                        
//                        if collision then
//                            blobList <- blobList
//                                |> List.filter (fun blob -> blob <> element || blob <> element')
//                                |> List.append [
//                                    {element with Velocity = element.Velocity * -1.f}
//                                    {element' with Velocity = element.Velocity * -1.f}
//                                ]
//                            
//                            if element <> element' && element.Lifetime > 25.f && element'.Lifetime > 25.f then
//                            blobList <- blobList
//                                |> List.append [{
//                                    Center = Utils.getRandomVector (0, Constants.WIDTH - 20, 0, Constants.HEIGHT - 20)
//                                    Radius = 10.f
//                                    Velocity = Utils.getRandomVector (-1, 2, -1, 2)
//                                    Color = Raylib.ColorAlpha (Color.GREEN, 1.f)
//                                    Type = BlobType.PassiveBlob
//                                    Lifetime = 100.f
//                                    Ticker = 0
//                                }]
//                    )
//                )
            
        let moveAndUpdateBlob list =
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
            
            let getColorAndLifetime (blob: Blob) =
                match blob.Type with
                    | PassiveBlob -> ((Color.GREEN, float32(blob.Lifetime - 0.05f) / 100.f), blob.Lifetime - 0.05f)
                    | AggroBlob -> ((Color.RED, float32(blob.Lifetime - 10.f) / 100.f), blob.Lifetime - 10.f)
                    | DiseasedBlob -> ((Color.YELLOW, float32(blob.Lifetime - 40.f) / 100.f), blob.Lifetime - 40.f)

            
            blobList <- list
                |> List.map (fun (element: Blob) ->
                    
                    let result = wallCollision element
                    let vX, vY =
                        if result <> Vector2 () then result.X, result.Y
                        elif element.Ticker % 120 = 0 then Utils.getRandomFloat(-1, 2), Utils.getRandomFloat(-1, 2) 
                        else element.Velocity.X, element.Velocity.Y
                    
                    let x = element.Center.X + vX
                    let y = element.Center.Y + vY
                    let color, lifetime = getColorAndLifetime element
                    {
                        element with
                            Center = Vector2 (x, y)
                            Velocity = Vector2 (vX, vY)
                            Color = Raylib.ColorAlpha color
                            Lifetime = lifetime
                    })
                |> List.filter (fun (element: Blob) ->
                    int(element.Lifetime) <> 0
                    )
                
        let updateFood list =
            foodList <- list
                |> List.map (fun (element: Food) ->
                    let color = (Color.DARKGRAY, float32(element.Lifetime - 0.1f / 100.f))
                    let lifetime = element.Lifetime - 0.1f
                    
                    {
                        element with
                            Color = Raylib.ColorAlpha color
                            Lifetime = lifetime
                    })

    module private Food =
        let createFood () =
            let food = {
                Center = Utils.getRandomVector (0, 1080, 0, 720)
                Radius = 5.f
                Color = Raylib.ColorAlpha (Color.DARKGRAY, 0.0f)
                Lifetime = 100.f
            }
            
            foodList <- food :: foodList
               
        let drawFood () =
            foodList
            |> List.map (fun element ->
                Raylib.DrawCircle (int(element.Center.X), int(element.Center.Y), element.Radius, element.Color))
    
    module private Blob =
        let initializeBlob () =
            let getBlob () =
                {
                    Center = Utils.getRandomVector (0, Constants.WIDTH - 20, 0, Constants.HEIGHT - 20)
                    Radius = 10.f
                    Velocity = Utils.getRandomVector (-1, 2, -1, 2)
                    Color = Raylib.ColorAlpha (Color.GREEN, 1.f)
                    Type = BlobType.PassiveBlob
                    Lifetime = 100.f
                    Ticker = 0
                }
            
            for _ in 0 .. 5 do
                blobList <- getBlob () :: blobList
        
        let drawBlobs () =
            blobList
            |> List.map (fun element -> 
                Raylib.DrawCircle (int(element.Center.X) ,int(element.Center.Y), element.Radius, element.Color))
        
    Blob.initializeBlob ()  
      
    let Setup () =
        // Update
        if (Raylib.IsKeyPressed KeyboardKey.KEY_SPACE) then paused <- not paused// Time period for food creation
        
        if not paused
        then
            if ticker % 50 = 0 then Food.createFood ()
            
            Update.updateTickerCounter ()
            Update.moveAndUpdateBlob blobList
            Update.updateFood foodList
            Update.onCollision ()
        
        // Draw
        Raylib.BeginDrawing ()
        Raylib.ClearBackground Color.BLACK
        
        if paused
        then
            Raylib.DrawText ("PAUSED", (Constants.WIDTH / 2) - 130, (Constants.HEIGHT / 2) - 50, 70, Color.GREEN)
        else
            Blob.drawBlobs ()
            Food.drawFood ()

            Raylib.DrawText ($"{blobList.Length}", Constants.WIDTH - 30, 10, 30, Color.GREEN)
                
        Raylib.EndDrawing ()
        ()
