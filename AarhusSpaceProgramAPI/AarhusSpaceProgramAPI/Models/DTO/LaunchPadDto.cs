namespace AarhusSpaceProgramAPI.Models;

public class LaunchPadDto
{
    public int LaunchPadId { get; set; }
    public string Location { get; set; }
    public double MaxWeight { get; set; }
    public string CurrentStatus { get; set; }
}