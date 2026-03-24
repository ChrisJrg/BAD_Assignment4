namespace AarhusSpaceProgramAPI.Models;

public class RocketDto
{
    public int RocketId { get; set; }
    public string Model { get; set; }
    public double Weight { get; set; }
    public int CrewCapacity { get; set; }
    public int Stages { get; set; }
    public double FuelCapacity { get; set; }
    public double PayloadCapacity { get; set; }
    
    public ICollection<Mission> Missions { get; set; } = new List<Mission>();
}