using Avalonia.Controls;
using Avalonia.Media;
using System ; 
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using System.Linq;
using System.Threading.Tasks;

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

        // Temps avant changement d'état
        private double _timeToMove = 0;

        // Forme de l'animal (utilise une image maintenant)
        public Image AnimalImage { get; set; }

        private Random _random = new Random();

        //Réserve d'energie
        public double EnergyReserve {get; set;}

        //Points de vie 
        public double PointsLife {get; set;}

        //Etat de l'animal : mort ou vivant (vivant par défaut)
        public bool IsAlive{get; private set;} = true;
        public bool IsCarnivore{get; set;}
        public bool IsHerbivore{get; set;}
        public bool IsFemale{get; set;}

        // Constructeur : je dois mettre en paramètre tout ce qui est nécessaire pour créer un animal : faire la différence entre carnivore et herbivore, males et femelles
        // Par défaut, un animal sera herbivore et mâle
        public Animals(int initialX, int initialY, Image animalImage, bool isCarnivore = false, bool isFemale = false )
        {
            XPosition = initialX;
            YPosition = initialY;
            AnimalImage = animalImage;
            IsCarnivore = isCarnivore;
            IsFemale = isFemale;

            XSpeed = _random.NextDouble() * 2 - 1;
            YSpeed = _random.NextDouble() * 2 - 1;

            // Initialiser la position de l'image sur le Canvas
            Canvas.SetLeft(AnimalImage, XPosition);
            Canvas.SetTop(AnimalImage, YPosition);

            //Fixer des valeurs pour les points de vie, et l'energie
            EnergyReserve = _random.Next(100, 101);
            PointsLife = _random.Next(30, 61);
        }

        //Pour gérer le déplacement, on a besoin des dimensions du canvas, et de la list de tous les animaux
        public void Move(double canvasWidth, double canvasHeight, List<Animals> allAnimals)
        {   
            //On parcourt la liste de tous les animaux 
            foreach (var otherAnimal in allAnimals)
            {
                // On s'assure qu'on compare pas un animal avec lui même
                if(otherAnimal != this)
                {
                    //Calculer la distance qui séparer deux animaux
                    double distance = Math.Sqrt(Math.Pow(this.XPosition - otherAnimal.XPosition, 2) + Math.Pow(this.YPosition - otherAnimal.YPosition, 2));
                    double thresholdDistance = 50;

                    //On fixe une distance de référence selon laquelle on va ajuster le comportemetn
                    if (distance < thresholdDistance)
                    {
                        //Si l'animal est carnivore et que l'autre animal est herbivore : valabe pour les mâles et femelles 
                        if ((this.IsCarnivore && otherAnimal.IsHerbivore)) 
                        {
                            //Le carnivore va poursuivre l'herbivore : on fait appel à la fonction pursue
                            PursueAnimal(otherAnimal, allAnimals);
                        }

                        //Faire en sorte que les femelles d'une même espèce s'évitent, et les mâles d'une même espèce s'évitent donc ajouter condition pour être sûre qu'on a le comportement attendu
                        if ((this.IsCarnivore && otherAnimal.IsCarnivore) || (this.IsHerbivore && otherAnimal.IsHerbivore)) 
                        {
                            //Eviter la collision
                            AvoidCollision(otherAnimal);
                        }
                    }
                }
            }
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

        //Poursuivre l'animal : on prend en paramètre la proie qui correspond à otheranimal dans la méthode move
        private void PursueAnimal(Animals prey, List<Animals> allAnimals)
        {
            // La durée de la poursuite (3 secondes)
            int pursueTime = 3; // en secondes, tu peux le rendre variable si besoin

            // Vérifier si le carnivore poursuit la proie
            if (pursueTime > 0)
            {
                // Réduire le temps de poursuite
                pursueTime -= 1;

                // Calculer la direction vers l'herbivore
                double deltaX = prey.XPosition - this.XPosition;
                double deltaY = prey.YPosition - this.YPosition;

                // Normaliser la direction pour que le carnivore se déplace correctement
                double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                if (distance > 0) // éviter la division par zéro
                {
                    deltaX /= distance;
                    deltaY /= distance;
                }

                // Déplacement du carnivore vers l'herbivore
                double moveSpeed = 0.1;  // Vitesse de déplacement, tu peux ajuster
                this.XPosition += deltaX * moveSpeed;
                this.YPosition += deltaY * moveSpeed;

                // Si le temps de poursuite est écoulé
                if (pursueTime <= 0)
                {
                    // L'herbivore disparaît
                    allAnimals.Remove(prey);  // Méthode pour faire disparaître l'herbivore (à définir)
                    Console.WriteLine("Un herbivore a été chassé");
                }
            }
            
        }

        private void AvoidCollision(Animals otherAnimal)
        {
            // Si une collision est détectée et qu'ils ne sont pas carnivores / herbivores, on inverse la direction
            XSpeed = -XSpeed;
            YSpeed = -YSpeed;

            // En option, on peut ajouter un petit facteur aléatoire pour rendre les mouvements moins prévisibles
            XSpeed += _random.NextDouble() * 0.2 - 0.1; // Ajuste légèrement la direction
            YSpeed += _random.NextDouble() * 0.2 - 0.1; // Ajuste légèrement la direction
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

        public static List<Animals> GenerateAnimals(int count, double canvasWidth, double canvasHeight, string imagePath, Canvas canvas, bool isCarnivore, bool isFemale)
        {
            var random = new Random();
            var animals = new List<Animals>();

            for (int i = 0; i < count; i++)
            {
                int x = (int)(random.NextDouble() * canvasWidth);
                int y = (int)(random.NextDouble() * canvasHeight);

                var animalImage = new Image
                {
                    Source = new Bitmap(imagePath),
                    Width = 80, 
                    Height = 80
                };

                canvas.Children.Add(animalImage);

                var animal = new Animals(x, y, animalImage, isCarnivore, isFemale);
                animals.Add(animal);
            }

            return animals;
        }

        public Meat? DieAndGenerateMeat(Canvas canvas)
        {
            IsAlive = false;
            canvas.Children.Remove(AnimalImage);
            Console.WriteLine("Un animal est mort");

            //Créer un morceau de viande à la position de l'animal
            var viande = Meat.Create(XPosition, YPosition, canvas);
            return viande;
        }

        public void Eat(List<Meat> viandes, Canvas canvas)
        {
            //Parcourir la liste de viande
            foreach (var viande in viandes.ToList())
            {
                if (!IsCarnivore) return ; //Si il n'est pas carnivore, fait rien

                double distance = Math.Sqrt(Math.Pow(this.XPosition - viande.XPosition, 2) + Math.Pow(this.YPosition - viande.YPosition, 2));

                if (distance < 50)
                {
                    this.EnergyReserve += 10;

                    viandes.Remove(viande);
                    canvas.Children.Remove(viande.MeatImage);
                    Console.WriteLine("L'animal a mangé la viande !");
                }
            }
        }

        public void UpdateMeatLifecycle(List<Meat> meats, List<Garbage> garbages, Canvas canvas)
        {
            foreach (var meat in meats.ToList()) // Utilise ToList() pour éviter les modifications de la liste pendant l'itération
            {
                var garbage = meat.CheckIfSpoiled(canvas);
                if (garbage != null)
                {
                    meats.Remove(meat);
                    garbages.Add(garbage);
                }
            }
        }
        
        

    

    }
}
