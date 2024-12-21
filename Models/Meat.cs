using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using System ;
using System.Net.NetworkInformation;

namespace Projet_ecosysteme.Models
{
    public class Meat 
    {
        public double XPosition {get; set;}
        public double YPosition {get; set;}
        public Image MeatImage {get; set;}

        //Durée avant que la viande devienne un déchet
        private const int TimeToDecay = 5000;
        private DateTime CreationTime {get; set;}

        public Meat(double x, double y, Image meatImage)
        {
            XPosition = x;
            YPosition = y;
            MeatImage = meatImage;
            CreationTime = DateTime.Now;
        }

        public static Meat Create(double x, double y, Canvas canvas)
        {
            var viandeImage = new Image
            {
                Source = new Bitmap("Assets/viande.png"),
                Width = 30, 
                Height = 30
            };

            canvas.Children.Add(viandeImage);
            Canvas.SetLeft(viandeImage, x);
            Canvas.SetTop(viandeImage, y);

            return new Meat(x, y, viandeImage);
        }

        public Garbage? CheckIfSpoiled(Canvas canvas)
        {
            if ((DateTime.Now - CreationTime).TotalMilliseconds >= TimeToDecay)
            {
                if (MeatImage != null)
                {
                    canvas.Children.Remove(MeatImage);
                }
                
                Console.WriteLine("La viande s'est transformée en déchet");
                return Garbage.Create(XPosition, YPosition, canvas);
            }

            return null;
        }
    }
}