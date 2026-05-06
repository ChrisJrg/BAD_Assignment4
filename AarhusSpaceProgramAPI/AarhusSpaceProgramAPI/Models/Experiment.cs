namespace AarhusSpaceProgramAPI.Models;

public class Experiment
{
    public int ExperimentId { get; set; }
    public string ExperimentName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int? ScientistId { get; set; }
    
    public Scientist Scientist { get; set; }
    
    public int? MissionId { get; set; }
    public Mission Mission { get; set; }
}