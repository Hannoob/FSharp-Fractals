namespace Clock

open System
open WebSharper
open WebSharper.JavaScript
open WebSharper.Html.Client

[<JavaScript>]
module Code =

    let AnimatedCanvas draw width height caption =
        let element = Canvas []
        let canvas  = As<CanvasElement> element.Dom
            
        canvas.Width  <- width
        canvas.Height <- height
        let ctx = canvas.GetContext "2d"
        draw ctx
       
        Div [ Width (string width); Attr.Style "float:left" ] -< [
            Div [ Attr.Style "float:center" ] -< [
                element
            ]
        ]

    let Main =
        let rnd = System.Random()
        let drawSquare (ctx: CanvasRenderingContext2D) size rotation =
            
            let angleL = rnd.Next (20,80)
            
            //It's big brain time!
            let LenR = Math.Sin(float(angleL) * Math.PI/180.) * size
            let LenL = Math.Sin(float(90-angleL) * Math.PI/180.) * size
            
            let xMid = Math.Cos(float(angleL) * Math.PI/180.) * LenL
            let yMid =  Math.Sin(float(angleL) * Math.PI/180.) * LenL  
            
            ctx.BeginPath()
            ctx.Rect( -size/2.,  -size, size, size)
//              ctx.LineTo((-size/2.) + xMid,(-size) - yMid)
            ctx.LineTo(size/2.,-size)
            ctx.Stroke()
            
            let newXL = (xMid - size)/2.
            let newYL = (-size) - (yMid/2.)
            let newRotationL = float(-angleL)
            
            let newXR = ((size/2.) + (xMid - (size/2.)))/2.
            let newYR = (-size) - (yMid/2.)
            let newRotationR = float(90 - angleL)
            
            ((newXL, newYL, LenL, newRotationL),(newXR, newYR, LenR, newRotationR))
        
        
        let rec drawTree (ctx: CanvasRenderingContext2D) depth x y size rotation =
            match depth with
            | 0 -> ()
            | d -> 
                ctx.Translate(x,y)
                ctx.Rotate(rotation * Math.PI/180.)
                let ((newXL, newYL, newSizeL, newRotationL),(newXR, newYR, newSizeR, newRotationR)) = drawSquare ctx size rotation
                
                drawTree ctx (d-1) newXL newYL newSizeL newRotationL
                
                drawTree ctx (d-1) newXR newYR newSizeR newRotationR
                
                ctx.Rotate(-rotation * Math.PI/180.)
                ctx.Translate(-x,-y)
        
        let example1 (ctx: CanvasRenderingContext2D) =
            let now = new Date()
            ctx.Save()
            ctx.Translate(300., 600.)
            ctx.StrokeStyle <- "black"
            ctx.FillStyle <- "white"
            ctx.LineWidth <- 1.
            ctx.Save()

            drawTree ctx 8 20. -100. 80. 0.
            
            ctx.Restore()

        Div [
            AnimatedCanvas example1 800 800 "1"
            Div [Attr.Style "clear:both"]
        ]
        |> fun e -> e.AppendTo "main"
