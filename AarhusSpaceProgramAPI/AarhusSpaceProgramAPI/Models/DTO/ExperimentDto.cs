namespace AarhusSpaceProgramAPI.Models.DTO;

public class ExperimentDto
{
    public int ExperimentId { get; set; }
    public string ExperimentName { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int? MissionId { get; set; }
    public int? ScientistId { get; set; }
}