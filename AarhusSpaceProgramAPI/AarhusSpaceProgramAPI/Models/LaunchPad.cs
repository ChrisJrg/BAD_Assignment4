namespace AarhusSpaceProgramAPI.Models;

public class LaunchPad
{
    public int LaunchPadId { get; set; }
    public string Location { get; set; }
    public double MaxWeight { get; set; }
    public string CurrentStatus { get; set; }
    
    public ICollection<Mission> Missions { get; set; } = new List<Mission>();
}