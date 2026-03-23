namespace AarhusSpaceProgramAPI.Models;

public class CelestialBody
{
    public int CelestialBodyId { get; set; }
    public int ParentId { get; set; }
    public string Name { get; set; }
    public double Distance { get; set; }
    public string Composition { get; set; }
    public string BodyType { get; set; }

    public CelestialBody ParentPlanet { get; set; }
    public ICollection<CelestialBody> Moons { get; set; } = new List<CelestialBody>();
    
    public ICollection<Mission> Missions { get; set; } = new List<Mission>();
}