namespace AarhusSpaceProgramAPI.Models;

public class MissionDto
{
    public int MissionId { get; set; }
    public string MissionName { get; set; }
    public DateTime LaunchDate { get; set; }
    public double Duration { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
    
    
    public int RocketId { get; set; }
    public int LaunchpPadId { get; set; }
    public int ManagerId { get; set; }
    public int TargetBodyId { get; set; }
    
    public Rocket Rocket { get; set; }
    public LaunchPad LaunchPad { get; set; }
    public Manager Manager { get; set; }
    public CelestialBody TargetBody { get; set; }

    
    
    
    public ICollection<Astronaut>  Astronauts { get; set; } = new List<Astronaut>();
    public ICollection<Scientist> Scientists { get; set; } = new List<Scientist>();

}