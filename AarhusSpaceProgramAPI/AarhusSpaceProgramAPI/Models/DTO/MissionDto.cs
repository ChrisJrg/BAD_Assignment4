namespace AarhusSpaceProgramAPI.Models;

public class MissionDto
{
    public int MissionId { get; set; }
    public string MissionName { get; set; }
    public DateTime LaunchDate { get; set; }
    public double Duration { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
    
    
    public int? RocketId { get; set; }
    public int? LaunchPadId { get; set; }
    public int? ManagerId { get; set; }
    public int? TargetBodyId { get; set; }
}