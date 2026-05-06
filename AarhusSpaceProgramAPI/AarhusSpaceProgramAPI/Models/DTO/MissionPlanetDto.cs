namespace AarhusSpaceProgramAPI.Models;

public class MissionPlanetDto
{
    public string MissionName { get; set; }
    
    public int? TargetBodyId { get; set; }
    
    public CelestialBody TargetBody { get; set; }
    


}