namespace AarhusSpaceProgramAPI.Models;

public class Rocket
{
    public int RocketId { get; set; }
    public string Model { get; set; }
    public double Weight { get; set; }
    public int CrewCapacity { get; set; }
    public int Stages { get; set; }
    public double FuelCapacity { get; set; }
    public double PayloadCapacity { get; set; }
}