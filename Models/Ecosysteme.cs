using System;
using System.Collections.Generic;
using System.Linq;

namespace Projet_ecosysteme.Models
{
    public class Ecosysteme
    {
        public List<Plante> Plantes { get; set; }
        private readonly Random random = new Random();
        private readonly object lockObject = new object(); // Verrou pour protéger la liste des plantes
        private HashSet<int> derniereReproductionIndexes = new HashSet<int>(); // Ensemble des index des plantes récemment reproduites, pour éviter les doublons

        public Ecosysteme()
        {
            Plantes = new List<Plante>();
        }

        public void AjouterPlante(Plante plante)
        {
            lock (lockObject) // Empêche les accès concurrents à la liste
            {
                // Ajoute la plante si elle respecte la distance minimale (10 pixels) avec les autres
                if (Plantes.All(p => CalculerDistance(plante.PositionX, plante.PositionY, p.PositionX, p.PositionY) >= 10))
                {
                    Plantes.Add(plante);
                }
            }
        }

        public void ReproduirePlantesAleatoires()
        {
            lock (lockObject) // Protège la liste des plantes pendant l'opération
            {
                // Si aucune plante n'est présente, arrêter
                if (Plantes.Count == 0) return;

                // Calcule le nombre de plantes à reproduire (2/3 du total)
                int nombreDePlantesAReproduire = Plantes.Count / 4;

                // Sélectionne les plantes éligibles (avec énergie suffisante, non récemment reproduites)
                var indicesDisponibles = Plantes
                    .Select((plante, index) => (plante, index))
                    .Where(p => p.plante.Energie > 5 && !derniereReproductionIndexes.Contains(p.index))
                    .Select(p => p.index)
                    .ToList();

                // Choisit aléatoirement parmi les plantes disponibles
                var indicesSelectionnes = indicesDisponibles.OrderBy(_ => random.Next()).Take(nombreDePlantesAReproduire).ToList();

                // Met à jour l'ensemble des plantes récemment reproduites
                derniereReproductionIndexes = new HashSet<int>(indicesSelectionnes);

                // Reproduit chaque plante sélectionnée
                foreach (int index in indicesSelectionnes)
                {
                    ReproduirePlante(Plantes[index]);
                }
            }
        }

        public void DiminuerEnergiePlantes()
        {
            lock (lockObject) // Protège la liste des plantes pendant l'opération
            {
                var plantesMortes = new List<Plante>();

                foreach (var plante in Plantes)
                {
                    if (plante.Vies > 0)
                    {
                        plante.Energie--; // Réduit l'énergie

                        if (plante.Energie <= 0)
                        {
                            plante.Vies--; // La plante perd une vie
                            plante.Energie = 10; // Réinitialise l'énergie
                        }
                    }
                    else // si la plante est morte
                    {
                        plante.TempsDepuisMort += 1;

                        if (plante.TempsDepuisMort >= 20) // La plante morte est retirée de la liste des plantes une fois qu'elle a atteint 20 secondes après sa mort.
                        {
                            plantesMortes.Add(plante); // Ajoute la plante morte à une liste temporaire pour suppression
                        }
                    }
                }

                // Supprime les plantes mortes de la liste principale
                foreach (var planteMorte in plantesMortes)
                {
                    Plantes.Remove(planteMorte);
                }
            }
        }

        public void ConsommerDechetsOrganiques()
        {
            lock (lockObject) // Protège la liste des plantes pendant l'opération
            {
                foreach (var plante in Plantes.Where(p => p.Vies > 0)) // Filtrer les plantes vivantes
                {
                    // Trouver les déchets organiques dans la zone de racines de la plante
                    var dechetsDansZone = Plantes
                        .Where(dechet => dechet.Vies == 0 && CalculerDistance(plante.PositionX, plante.PositionY, dechet.PositionX, dechet.PositionY) <= plante.ZoneDeRacines)
                        .ToList(); // Déchets organiques (plantes mortes)

                    foreach (var dechet in dechetsDansZone)
                    {
                        plante.Energie += 5; // La plante regagne de l'énergie
                        if (plante.Energie > 10) plante.Energie = 10;

                        Plantes.Remove(dechet);
                    }
                }
            }
        }

        private void ReproduirePlante(Plante plante)
        {
            int tentative = 0;
            bool positionValide = false;
            int nouvellePositionX = 0;
            int nouvellePositionY = 0;

            // Génère une nouvelle position valide dans un rayon défini
            while (!positionValide && tentative < 10)
            {
                nouvellePositionX = plante.PositionX + GetRandomOffset(plante.RayonSemis);
                nouvellePositionY = plante.PositionY + GetRandomOffset(plante.RayonSemis);

                // S'assure que la nouvelle position est dans les limites
                nouvellePositionX = Math.Clamp(nouvellePositionX, 0, 800);
                nouvellePositionY = Math.Clamp(nouvellePositionY, 0, 450);

                // Vérifie que la distance minimale avec les autres plantes est respectée
                positionValide = Plantes.All(p => CalculerDistance(nouvellePositionX, nouvellePositionY, p.PositionX, p.PositionY) >= 10);

                tentative++;
            }

            // Si une position valide a été trouvée, créer une nouvelle plante
            if (positionValide)
            {
                Plante nouvellePlante = new Plante(nouvellePositionX, nouvellePositionY, 10, plante.RayonSemis);
                AjouterPlante(nouvellePlante);
            }
        }

        // Génère un déplacement aléatoire dans un rayon donné
        private int GetRandomOffset(int rayon)
        {
            return random.Next(-rayon, rayon + 1);
        }

        private double CalculerDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
    }
}
