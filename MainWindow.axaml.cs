using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Projet_ecosysteme.Models;
using Avalonia.Media.Imaging;

namespace Projet_ecosysteme
{
    public partial class MainWindow : Window
    {
        private List<Animals> _animals_carnivores = new List<Animals>();
        private List<Animals> _animals_herbivores = new List<Animals>();
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

            // Créer des carnivores (ex. Puma)
            for (int i = 0; i < 5; i++)
            {
                int x = (int)(_random.NextDouble() * canvasWidth);
                int y = (int)(_random.NextDouble() * canvasHeight);

                // Créer une Image pour l'animal carnivore
                var carnivoreImage = new Image
                {
                    Source = new Bitmap("Assets/Puma.png"), // Utilisez le chemin en string
                    Width = 80,
                    Height = 80
                };

                MyCanvas.Children.Add(carnivoreImage);

                var animal_carnivore = new Animals(x, y, carnivoreImage);
                _animals_carnivores.Add(animal_carnivore);
            }

            // Créer des herbivores (ex. Biche)
            for (int i = 0; i < 5; i++)
            {
                int x = (int)(_random.NextDouble() * canvasWidth);
                int y = (int)(_random.NextDouble() * canvasHeight);

                // Créer une Image pour l'animal herbivore
                var herbivoreImage = new Image
                {
                    Source = new Bitmap("Assets/mouton.png"), // Utilisez le chemin en string
                    Width = 80,
                    Height = 80
                };

                MyCanvas.Children.Add(herbivoreImage);

                var animal_herbivore = new Animals(x, y, herbivoreImage);
                _animals_herbivores.Add(animal_herbivore);
            }

            // Déplacer les animaux périodiquement
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            timer.Tick += (sender, args) =>
            {
                foreach (var animal in _animals_carnivores)
                {
                    animal.Move(canvasWidth, canvasHeight);
                }

                foreach (var animal in _animals_herbivores)
                {
                    animal.Move(canvasWidth, canvasHeight);
                }
            };
            timer.Start();
        }
    }
}
