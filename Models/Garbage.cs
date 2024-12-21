using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using System ;
using System.Net.NetworkInformation;

namespace Projet_ecosysteme.Models
{
    public class Garbage 
    {
        public double XPosition {get; set;}
        public double YPosition {get; set;}
        public Image GarbageImage {get; set;}

        public Garbage(double x, double y, Image garbageImage)
        {
            XPosition = x;
            YPosition = y;
            GarbageImage = garbageImage;
        }

        public static Garbage Create(double x, double y, Canvas canvas)
        {
            var garbageImage = new Image
            {
                Source = new Bitmap("Assets/DechetOrg.png"),
                Width = 30, 
                Height = 30
            };

            canvas.Children.Add(garbageImage);
            Canvas.SetLeft(garbageImage, x);
            Canvas.SetTop(garbageImage, y);

            return new Garbage(x, y, garbageImage);
        }
    }
}