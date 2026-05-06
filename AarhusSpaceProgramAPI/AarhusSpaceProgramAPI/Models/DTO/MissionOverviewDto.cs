namespace AarhusSpaceProgramAPI.Models;

public class MissionOverviewDto
{
    public string MissionName { get; set; }
    public DateTime LaunchDate { get; set; }
    public string ManagerName { get; set; }
    public string RocketModel { get; set; }
    public string LaunchPadLocation { get; set; }
    public string TargetBodyName { get; set; }
    
}