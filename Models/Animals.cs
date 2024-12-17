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
        public double EnergyReserve {get; set;}

        //Points de vie 
        public double PointsLife {get; set;}

        //Etat de l'animal : mort ou vivant (vivant par défaut)
        public bool IsAlive{get; private set;} = true;

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

            //Fixer des valeurs pour les points de vie, et l'energie
            EnergyReserve = _random.Next(50, 101);
            PointsLife = _random.Next(30, 61);
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

        //Mise à jour du cycle de vie
        public void UpdateLifeCycle(){

            //Si l'animal est mort, on fait rien pour l'instant
            if (!IsAlive) return ; 

            //Réducation progressive de l'energie
            EnergyReserve -= 0.1;

            if (EnergyReserve <= 0){
                //Si y'a plus d'energie, convertir les points en energie
                //Il faudra modifier la logique de calcul ici 
                EnergyReserve = 0;
                PointsLife -= 0.5;
            }

            //Une fois que y'a plus de points de vie, l'animal meurt
            if (PointsLife <= 0){
                Die(); //On fait appelle à la fonction die 
            }
        }

        //Faire en sorte que l'animal disparaissent une fois que il a plus de points de vies
        public void Die(){

            IsAlive = false ; 
            Console.WriteLine("Un animal est mort");
        }
    }
}
