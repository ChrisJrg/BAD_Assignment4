namespace AarhusSpaceProgramAPI.Models;

public class Mission
{
    public int MissionId { get; set; }
    public string MissionName { get; set; }
    public DateTime LaunchDate { get; set; }
    public double Duration { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
}