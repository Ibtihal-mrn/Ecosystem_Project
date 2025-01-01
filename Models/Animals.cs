using Avalonia.Controls;
using System ; 
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using System.Linq;


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

        //Réserve d'energie et points de vies
        public double EnergyReserve {get; set;}
        public double PointsLife {get; set;}

        //Etat de l'animal : mort ou vivant (vivant par défaut) et carnivore ou herbivore
        public bool IsAlive{get; private set;} = true;
        public bool IsCarnivore{get; set;}

        // Par défaut, un animal sera herbivore
        public Animals(int initialX, int initialY, Image animalImage, bool isCarnivore = false)
        {
            XPosition = initialX;
            YPosition = initialY;
            AnimalImage = animalImage;
            IsCarnivore = isCarnivore;
            
            XSpeed = _random.NextDouble() * 2 - 1;
            YSpeed = _random.NextDouble() * 2 - 1;

            // Initialiser la position de l'image sur le Canvas
            Canvas.SetLeft(AnimalImage, XPosition);
            Canvas.SetTop(AnimalImage, YPosition);

            //Fixer des valeurs pour les points de vie, et l'energie
            EnergyReserve = _random.Next(100, 200);
            PointsLife = _random.Next(30, 60);
        }

        //Pour gérer le déplacement, on a besoin des dimensions du canvas, et de la liste de tous les animaux
        public void Move(double canvasWidth, double canvasHeight, List<Animals> allAnimals)
        {   
            //On parcourt la liste de tous les animaux 
            foreach (var otherAnimal in allAnimals)
            {
                // On s'assure qu'on compare pas un animal avec lui même
                if(otherAnimal != this)
                {
                    //Calculer la distance qui séparer deux animaux ==> Gestion de la zone
                    double distance = Math.Sqrt(Math.Pow(this.XPosition - otherAnimal.XPosition, 2) + Math.Pow(this.YPosition - otherAnimal.YPosition, 2));
                    double thresholdDistance = 30;

                    //On fixe une distance de référence selon laquelle on va ajuster le comportemetn
                    if (distance < thresholdDistance)
                    {
                        //Si l'animal est carnivore et que l'autre animal est herbivore : valabe pour les mâles et femelles 
                        if (this.IsCarnivore && !otherAnimal.IsCarnivore)
                        {
                            Console.WriteLine("Carnivore détecte herbivore à proximité");
                            //Le carnivore va chasser l'herbivore
                            HuntAnimal(otherAnimal);
                        }
                    }
                }
            }

            // La position est mise à jour en fonction de la vitesse 
            XPosition += XSpeed;
            YPosition += YSpeed;

            // Permet de gérer les bords de la fenêtre pour éviter que l'animal ne sorte
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

        
        //Chasser l'herbivore
        private void HuntAnimal(Animals prey)
        {
            if (!prey.IsAlive) return; // Si la proie est déjà morte, ne rien faire

            // Le carnivore mange l'herbivore
            prey.Die();  // L'herbivore meurt

            // Le carnivore gagne de l'énergie ou des points de vie
            this.EnergyReserve += 20;  // Par exemple, on peut ajouter de l'énergie au carnivore
            this.PointsLife += 5;      // Ajouter des points de vie

            Console.WriteLine("Le carnivore a mangé l'herbivore et a gagné de l'énergie !");
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
                EnergyReserve = EnergyReserve;
                PointsLife -= 0.5;
            }

            //Une fois que y'a plus de points de vie, l'animal meurt
            if (PointsLife <= 0){
                Die(); //On fait appelle à la fonction die 
            }
        }

        //Faire en sorte que l'animal disparaissent une fois que il a plus de points de vies
        private void Die(){

            IsAlive = false ; 
            Console.WriteLine("Un animal est mort");
        }

        public static List<Animals> GenerateAnimals(int count, double canvasWidth, double canvasHeight, string imagePath, Canvas canvas, bool isCarnivore)
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

                var animal = new Animals(x, y, animalImage, isCarnivore);
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

        public void EatPlante(List<Plante> plantes)
        {
            // Parcourir la liste de viande
            foreach (var plante in plantes.ToList())
            {
                if (IsCarnivore) return ; //Si il n'est pas carnivore, fait rien

                double distance = Math.Sqrt(Math.Pow(this.XPosition - plante.PositionX, 2) + Math.Pow(this.YPosition - plante.PositionY, 2));

                if (distance < 50)
                {
                    if(plante.Vies > 0)
                    {
                        this.EnergyReserve += 10;
                        plantes.Remove(plante);
                    }
                    Console.WriteLine("L'animal a mangé une plante!");
                }
            }
        }
        
        
    }
}
