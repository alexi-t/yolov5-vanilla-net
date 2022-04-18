using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Graphics.Skia;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models;

namespace Yolov5Net.App
{
    class Program
    {
        static void Main(string[] args)
        {
            using var fs = new FileStream("Assets/test.jpg", FileMode.Open);
            using var image = SkiaImage.FromStream(fs, Microsoft.Maui.Graphics.ImageFormat.Jpeg);

            var (width, height) = ((int)image.Width, (int)image.Height);

            using var scorer = new YoloScorer<YoloCocoP5Model>("Assets/Weights/yolov5n.onnx");

            List<YoloPrediction> predictions = scorer.Predict(image);

            using var ctx = new PlatformBitmapExportService().CreateContext(width, height);

            var graphics = ctx.Canvas;
            graphics.DrawImage(image, 0, 0, width, height);

            foreach (var prediction in predictions) // iterate predictions to draw results
            {
                double score = Math.Round(prediction.Score, 2);


                graphics.StrokeColor = prediction.Label.Color;
                graphics.DrawRectangle(prediction.Rectangle.X, prediction.Rectangle.Y,
                    prediction.Rectangle.Width, prediction.Rectangle.Height);

                var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);

                graphics.FontColor = prediction.Label.Color;
                graphics.DrawString($"{prediction.Label.Name} ({score})",
                    x, y, 
                    Microsoft.Maui.Graphics.HorizontalAlignment.Left);
            }

            graphics.SaveState();

            using var output = new FileStream("Assets/result.jpg", FileMode.Create);
            ctx.WriteToStream(output);
            output.Flush();
        }
    }
}
