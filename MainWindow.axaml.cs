using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Threading;
using Projet_ecosysteme.Models;
using System.Linq;
using System.Timers;
using Avalonia.Media.Imaging;

namespace Projet_ecosysteme
{
    public partial class MainWindow : Window
    {
        private List<Animals> _animals_carnivores = new List<Animals>();
        private List<Animals> _animals_herbivores = new List<Animals>();
        private List<Meat> _viande = new List<Meat>();
        private List<Garbage> _garbage = new List<Garbage>();
        private Projet_ecosysteme.Models.Ecosysteme ecosysteme;
        private Bitmap planteImage;
        private Bitmap planteEnDechetOrgImage;
        private readonly Random random = new Random();
        private Timer? reproductionTimer;
        private Timer? energieTimer;

        public MainWindow()
        {
            InitializeComponent();

            // Abonnement à l'événement LayoutUpdated pour attendre que le Canvas soit initialisé
            this.LayoutUpdated += InitializeAnimalsOnLayoutUpdate;

            ecosysteme = new Projet_ecosysteme.Models.Ecosysteme();
            planteImage = new Bitmap("Assets/plante.png");
            planteEnDechetOrgImage = new Bitmap("Assets/DechetOrg.png");

            // Événement déclenché à l'ouverture de la fenêtre
            this.Opened += (sender, e) =>
            {
                InitialiserEcosysteme();
                MettreAJourAffichage();

                reproductionTimer = new Timer(2000);
                reproductionTimer.Elapsed += OnReproductionTimerElapsed;
                reproductionTimer.Start();

                energieTimer = new Timer(500);
                energieTimer.Elapsed += OnEnergieTimerElapsed;
                energieTimer.Start();
            };
        }

        private void InitialiserEcosysteme()
        {
            // Obtenir la largeur et la hauteur actuelles de la fenêtre
            double canvasWidth = MyCanvas.Bounds.Width;
            double canvasHeight = MyCanvas.Bounds.Height;

            while (ecosysteme.Plantes.Count < 20)
            {
                int positionX = random.Next(0, (int)canvasWidth);
                int positionY = random.Next(0, (int)canvasHeight);
                var plante = new Plante(positionX, positionY, 10, 50);
                ecosysteme.AjouterPlante(plante);
            }
        }

        private void OnReproductionTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ecosysteme.ReproduirePlantesAleatoires();
                MettreAJourAffichage();
            });
        }

        private void OnEnergieTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ecosysteme.DiminuerEnergiePlantes();
                ecosysteme.ConsommerDechetsOrganiques();
                MettreAJourAffichage();
            });
        }

        private void MettreAJourAffichage()
        {
            // Supprimer uniquement les images correspondant aux plantes
            var plantesImages = MyCanvas.Children.OfType<Image>()
                .Where(img => img.Source == planteImage || img.Source == planteEnDechetOrgImage)
                .ToList();

            foreach (var image in plantesImages)
            {
                MyCanvas.Children.Remove(image);
            }

            // Ajouter les plantes au Canvas
            foreach (var plante in ecosysteme.Plantes)
            {
                var image = new Image
                {
                    Source = plante.Vies > 0 ? planteImage : planteEnDechetOrgImage,
                    Width = plante.Vies > 0 ? 50 : 30,
                    Height = plante.Vies > 0 ? 50 : 30
                };

                Canvas.SetLeft(image, plante.PositionX);
                Canvas.SetTop(image, plante.PositionY);

                // Ajouter l'image des plantes au début pour simuler un arrière-plan
                MyCanvas.Children.Insert(0, image);
            }
        }

        private void InitializeAnimalsOnLayoutUpdate(object? sender, EventArgs e)
        {
            if (MyCanvas.Bounds.Width > 0 && MyCanvas.Bounds.Height > 0)
            {
                this.LayoutUpdated -= InitializeAnimalsOnLayoutUpdate;
                InitializeAnimals();
            }
        }

        private void InitializeAnimals()
        {
            double canvasWidth = MyCanvas.Bounds.Width;
            double canvasHeight = MyCanvas.Bounds.Height;

            // Créer des carnivores
            _animals_carnivores = Animals.GenerateAnimals(5, canvasWidth, canvasHeight, "Assets/Puma.png", MyCanvas, true);

            // Créer des herbivores
            _animals_herbivores = Animals.GenerateAnimals(15, canvasWidth, canvasHeight, "Assets/mouton.png", MyCanvas, false);

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
                animal.Move(MyCanvas.Bounds.Width, MyCanvas.Bounds.Height, _animals_carnivores.Concat(_animals_herbivores).ToList());
                animal.UpdateLifeCycle();

                // Manger la viande si assez proche
                animal.Eat(_viande, MyCanvas);
                animal.EatPlante(ecosysteme.Plantes, MyCanvas);
                

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

            // Vérification de la viande pour la transformer en déchet
            foreach (var viande in _viande.ToList())
            {
                var garbage = viande.CheckIfSpoiled(MyCanvas);
                if (garbage != null)
                {
                    _viande.Remove(viande);
                    _garbage.Add(garbage);
                }
            }
        }
    }
}
