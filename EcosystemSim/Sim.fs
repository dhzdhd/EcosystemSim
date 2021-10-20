namespace EcoSim

open System
open System.Numerics
open EcoSim
open Microsoft.VisualBasic.CompilerServices
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
        time: int
        passive: int
        aggressive: int
        diseased: int
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
    let mutable blobCount = List.empty

    module private Update =
        let updateTickerCounter () =
            secondTicker <- secondTicker + (1. / 60.)  // Update counter as 60 frames = 1 second
            ticker <- ticker + 1   // Update counter per frame
            
            blobList <- blobList
            |> List.map (fun element ->
                {element with Ticker = element.Ticker + 1})
            
        let onFoodCollision () =
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
            
        let onBlobCollision () =
//          Blob and blob
            blobList
            |> List.iter (fun (element: Blob) ->
                blobList
                    |> List.iter (fun (element': Blob) ->
                        let collision =
                            Raylib.CheckCollisionCircles (element.Center, element.Radius, element'.Center, element'.Radius)
                            && element <> element'
                        let vel = Vector2(element'.Velocity.X, element'.Velocity.Y)
                        let vel' = Vector2(element.Velocity.X, element.Velocity.Y)
                        
                        if collision then
                            blobList <- blobList
                                |> List.filter (fun blob -> blob.Center <> element.Center)
                                |> List.filter (fun blob -> blob.Center <> element'.Center)
                                |> List.append [{element with Velocity = vel}; {element' with Velocity = vel'}]
                            
//                            if element <> element' && element.Lifetime > 25.f && element'.Lifetime > 25.f then
//                            blobList <- blobList
//                                |> List.append [{
//                                    Center = Utils.getRandomVector (0, Constants.WIDTH - 20, 0, Constants.HEIGHT - 20)
//                                    Radius = 10.f
//                                    Velocity = Utils.getRandomVector (-1, 2, -1, 2)
//                                    Color = Raylib.ColorAlpha (Constants.PASSIVE_COLOR, 1.f)
//                                    Type = BlobType.PassiveBlob
//                                    Lifetime = 100.f
//                                    Ticker = 0
//                                }]
                    )
                )
            
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
                    | PassiveBlob -> ((Constants.PASSIVE_COLOR, float32(blob.Lifetime - 0.05f) / 100.f), blob.Lifetime - 0.05f)
                    | AggroBlob -> ((Constants.AGGRO_COLOR, float32(blob.Lifetime - 0.025f) / 100.f), blob.Lifetime - 0.025f)
                    | DiseasedBlob -> ((Constants.DISEASED_COLOR, float32(blob.Lifetime - 0.20f) / 100.f), blob.Lifetime - 0.20f)

            
            blobList <- list
                |> List.map (fun (element: Blob) ->
                    
                    let result = wallCollision element
                    let vX, vY =
                        if result <> Vector2 () then result.X, result.Y  // If wall collision
                        // If no wall collision but random movement generation
                        elif element.Ticker % 120 = 0 then Utils.getRandomFloat(-1, 2), Utils.getRandomFloat(-1, 2) 
                        else element.Velocity.X, element.Velocity.Y  // If no wall collision
                    
                    let x = element.Center.X + vX
                    let y = element.Center.Y + vY
                    let color, lifetime = getColorAndLifetime element
                    {  // Update list with new parameters
                        element with
                            Center = Vector2 (x, y)
                            Velocity = Vector2 (vX, vY)
                            Color = Raylib.ColorAlpha color
                            Lifetime = lifetime
                    })
                |> List.filter (fun (element: Blob) ->
                    int(element.Lifetime) <> 0  // Filter out dead blobs
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
        let createBlob (blobType: Option<_>) =
            let colorList = [Constants.PASSIVE_COLOR; Constants.AGGRO_COLOR; Constants.DISEASED_COLOR]
            let blobTypeList = [BlobType.PassiveBlob; BlobType.AggroBlob; BlobType.DiseasedBlob]
            let random = Random ()
            let random = random.Next (0, 3)
            
            let color, blobType' =
                match blobType with
                    | Some("passive") -> colorList.[0], blobTypeList.[0]
                    | Some("aggro") -> colorList.[1], blobTypeList.[1]
                    | Some("diseased") -> colorList.[2], blobTypeList.[2]
                    | Some _
                    | None -> colorList.[random], blobTypeList.[random]
                        
            {
                Center = Utils.getRandomVector (0, Constants.WIDTH - 20, 0, Constants.HEIGHT - 20)
                Radius = 10.f
                Velocity = Utils.getRandomVector (-1, 2, -1, 2)
                Color = Raylib.ColorAlpha (color, 1.f)
                Type = blobType'
                Lifetime = 100.f
                Ticker = 0
            }
        
        // Create a set number of blobs when the sim starts.
        let initializeBlob () =
            for _ in 0 .. Constants.INIT_BLOB_AMOUNT do
                blobList <- createBlob (Option.Some "passive") :: blobList
                blobCount <- blobCount
                    |> List.append [{
                         time = ticker
                         passive =
                             if blobCount.IsEmpty
                             then 1
                             else blobCount.[blobCount.Length - 1].passive + 1
                         aggressive = 0
                         diseased = 0
                     }]
                
                
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
            if ticker % Constants.INIT_FOOD_TIMER = 0 then Food.createFood ()
            printfn $"{blobCount}"
            
            if Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON) then
                let blob = Blob.createBlob None
                blobList <- blob :: blobList
            
            Update.updateTickerCounter ()
            Update.moveAndUpdateBlob blobList
            Update.updateFood foodList
            Update.onFoodCollision ()
            Update.onBlobCollision ()
        
        // Draw
        Raylib.BeginDrawing ()
        Raylib.ClearBackground Color.BLACK
        
        if paused
        then
            Raylib.DrawText ("PAUSED", (Constants.WIDTH / 2) - 130, (Constants.HEIGHT / 2) - 50, 70, Constants.PASSIVE_COLOR)
        else
            Blob.drawBlobs ()
            Food.drawFood ()

            Raylib.DrawText ($"{blobList.Length}", Constants.WIDTH - 50, 10, 30, Constants.PASSIVE_COLOR)
                
        Raylib.EndDrawing ()
        ()
