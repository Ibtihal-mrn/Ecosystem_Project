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
        private List<Animals> _animals_herbivores = new List<Animals>();
        private List<Meat> _viande = new List<Meat>(); //Créer une liste dans laquelle on va transformer les animaux en viande
        private Random _random = new Random();

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
            _animals_carnivores = Animals.GenerateAnimals(5, canvasWidth, canvasHeight, "Assets/Puma.png", MyCanvas);


            //Créer des herbivores
            _animals_herbivores = Animals.GenerateAnimals(5, canvasWidth, canvasHeight, "Assets/mouton.png", MyCanvas);

            // Déplacer les animaux périodiquement
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            timer.Tick += (sender, args) =>
            {
                UpdateAnimals(_animals_carnivores);
                UpdateAnimals(_animals_herbivores);          
            };
            timer.Start();
        }

        private void UpdateAnimals(List<Animals> animals)
        {
            foreach (var animal in animals.ToList())
            {
                animal.Move(MyCanvas.Bounds.Width, MyCanvas.Bounds.Height);
                animal.UpdateLifeCycle();

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
        }
    }
}
