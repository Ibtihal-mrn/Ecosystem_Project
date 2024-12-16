using Avalonia.Controls;
using Avalonia.Media;
using System ; 

namespace Projet_ecosysteme.Models
{
    public class Animals
    {
        // Positions de l'animal
        public double XPosition { get; set; }
        public double YPosition { get; set; }

        // Vitesse de l'animal
        public double XSpeed { get; set; }
        public double YSpeed { get; set; }

        // Forme de l'animal (utilise une image maintenant)
        public Image AnimalImage { get; set; }

        private Random _random = new Random();

        //Réserve d'energie
        public double energy_reserve {get; set;}

        //Points de vie 
        public double points_life {get; set;}

        // Constructeur
        public Animals(int initialX, int initialY, Image animalImage)
        {
            XPosition = initialX;
            YPosition = initialY;
            AnimalImage = animalImage;

            XSpeed = _random.NextDouble() * 2 - 1;
            YSpeed = _random.NextDouble() * 2 - 1;

            // Initialiser la position de l'image sur le Canvas
            Canvas.SetLeft(AnimalImage, XPosition);
            Canvas.SetTop(AnimalImage, YPosition);
        }

        public void Move(double canvasWidth, double canvasHeight)
        {
            // La position est mise à jour en fonction de la vitesse 
            XPosition += XSpeed;
            YPosition += YSpeed;

            // Permet de gérer les bords de la fenêtre pour éviter que l'animal ne sorte
            // Dans mon cas, l'animal rebondit, mais je peux changer la logique pour le faire réapparaître de l'autre côté.
            if (XPosition <= 0 || XPosition >= canvasWidth - AnimalImage.Width)
            {
                XSpeed = -XSpeed;
                XPosition = Math.Clamp(XPosition, 0, canvasWidth - AnimalImage.Width);
            }

            if (YPosition <= 0 || YPosition >= canvasHeight - AnimalImage.Height)
            {
                YSpeed = -YSpeed;
                YPosition = Math.Clamp(YPosition, 0, canvasHeight - AnimalImage.Height);
            }

            // Mettre à jour la position de l'image sur le Canvas
            Canvas.SetLeft(AnimalImage, XPosition);
            Canvas.SetTop(AnimalImage, YPosition);
        }

        public void Die(){
            
        }
    }
}
