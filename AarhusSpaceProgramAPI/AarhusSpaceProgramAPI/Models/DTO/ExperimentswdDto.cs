namespace AarhusSpaceProgramAPI.Models.DTO;

public class ExperimentwdDto
{
    public int ExperimentId { get; set; }
    public string ExperimentName { get; set; }
    public string Description { get; set; }
    
    public int? MissionId { get; set; }
    public int? ScientistId { get; set; }
}