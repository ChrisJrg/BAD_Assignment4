namespace AarhusSpaceProgramAPI.Models.DTO;

public class MissionExperimentsDto
{
    public string MissionName { get; set; }
    
    public ICollection<Experiment> Experiments { get; set; } = new List<Experiment>();

}
    
    

