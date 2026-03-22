namespace AarhusSpaceProgramAPI.Models;

public class LaunchPad
{
    public int LaunchPadId { get; set; }
    public string Location { get; set; }
    public double MaxWeight { get; set; }
    public string CurrentStatus { get; set; }
}