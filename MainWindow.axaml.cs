using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Projet_ecosysteme.Models;
using Avalonia.Media.Imaging;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace Projet_ecosysteme
{
    public partial class MainWindow : Window
    {
        private List<Animals> _animals_carnivores = new List<Animals>();
        private List<Animals> _animals_carnivores_femelles = new List<Animals>();
        private List<Animals> _animals_herbivores = new List<Animals>();
        private List<Animals> _animals_herbivores_femelles = new List<Animals>();
        private List<Meat> _viande = new List<Meat>(); //Créer une liste dans laquelle on va transformer les animaux en viande
        private Random _random = new Random();
        private List<Garbage> _garbage = new List<Garbage>();

        public MainWindow()
        {
            InitializeComponent();

            // Abonnement à l'événement LayoutUpdated pour attendre que le Canvas soit initialisé
            this.LayoutUpdated += InitializeAnimalsOnLayoutUpdate;
        }

        private void InitializeAnimalsOnLayoutUpdate(object? sender, EventArgs e)
        {
            if (MyCanvas.Bounds.Width > 0 && MyCanvas.Bounds.Height > 0)
            {
                this.LayoutUpdated -= InitializeAnimalsOnLayoutUpdate; // Désabonnement après l'initialisation
                InitializeAnimals(); // Appel à la méthode d'initialisation des animaux
            }
        }

        private void InitializeAnimals()
        {
            double canvasWidth = MyCanvas.Bounds.Width;
            double canvasHeight = MyCanvas.Bounds.Height;

            //Créer des carnivores
            _animals_carnivores = Animals.GenerateAnimals(5, canvasWidth, canvasHeight, "Assets/Puma.png", MyCanvas, true, false);
            _animals_carnivores_femelles = Animals.GenerateAnimals(5, canvasWidth, canvasHeight, "Assets/chat.png", MyCanvas, true, false);
            

            //Créer des herbivores
            _animals_herbivores = Animals.GenerateAnimals(5, canvasWidth, canvasHeight, "Assets/mouton.png", MyCanvas, false, false);
            _animals_herbivores_femelles = Animals.GenerateAnimals(5, canvasWidth, canvasHeight, "Assets/biche.png", MyCanvas, false, true);

            // Déplacer les animaux périodiquement
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            timer.Tick += (sender, args) =>
            {
                UpdateAnimals(_animals_carnivores);
                UpdateAnimals(_animals_carnivores_femelles);
                UpdateAnimals(_animals_herbivores); 
                UpdateAnimals(_animals_herbivores_femelles);        
            };
            timer.Start();
        }

        private void UpdateAnimals(List<Animals> animals)
        {
            foreach (var animal in animals.ToList())
            {
                animal.Move(MyCanvas.Bounds.Width, MyCanvas.Bounds.Height, _animals_carnivores.Concat(_animals_carnivores_femelles).Concat(_animals_herbivores).Concat(_animals_herbivores_femelles).ToList() );
                animal.UpdateLifeCycle();

                //Manger la viande si assez proche
                animal.Eat(_viande, MyCanvas); //On passe la liste de viande à la fonction qui doit justement prendre une liste en paramètre


                if (!animal.IsAlive)
                {
                    var viande = animal.DieAndGenerateMeat(MyCanvas);
                    if (viande != null)
                    {
                        _viande.Add(viande);
                    }

                    animals.Remove(animal);
                }
            }

            //Vérification de la viande pour la tranformer en déchet
            foreach (var viande in _viande.ToList())
            {
                var garbage = viande.CheckIfSpoiled(MyCanvas);
                if (garbage != null)
                {
                    _garbage.Add(garbage);
                }
            }
        }
    }
}
