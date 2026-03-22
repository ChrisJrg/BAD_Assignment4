namespace AarhusSpaceProgramAPI.Models;

public class Astronaut
{
    public int AstronautId  { get; set; }
    public string Name  { get; set; }
    public DateTime HireDate  { get; set; }
    public double PayGrade { get; set; }
    public string Rank { get; set; }
    public double EXPInSim { get; set; }
    public double EXPInSpace { get; set; }
    
}