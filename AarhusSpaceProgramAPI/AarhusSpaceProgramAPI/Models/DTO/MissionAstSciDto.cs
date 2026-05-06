namespace AarhusSpaceProgramAPI.Models;

public class MissionAstSciDto
{
    public string MissionName { get; set; }
    
    public ICollection<Astronaut>  Astronauts { get; set; } = new List<Astronaut>();
    public ICollection<Scientist> Scientists { get; set; } = new List<Scientist>();


}