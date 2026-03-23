namespace AarhusSpaceProgramAPI.Models;

public class Scientist
{
    public int ScientistId { get; set; }
    public string Name { get; set; }
    public DateTime HireDate { get; set; }
    public string Title {get; set;}
    public string Specialty { get; set; }
    
    public ICollection<Mission> Missions { get; set; } =  new List<Mission>();
}