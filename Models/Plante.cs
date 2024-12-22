namespace Projet_ecosysteme.Models
{
    public class Plante
    {
        public int Energie { get; set; }
        public int Vies { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int RayonSemis { get; set; }
        public int ZoneDeRacines { get; set; } // Rayon pour la consommation des déchets
        public double TempsDepuisMort { get; set; } // Temps écoulé depuis la mort en secondes

        public Plante(int x, int y, int energie, int rayonSemis)
        {
            PositionX = x;
            PositionY = y;
            Energie = energie;
            Vies = 3;
            RayonSemis = rayonSemis;
            ZoneDeRacines = 50; // Rayon par défaut pour la zone de racines
            TempsDepuisMort = 0;
        }
    }
}
