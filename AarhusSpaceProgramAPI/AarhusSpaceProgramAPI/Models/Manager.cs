namespace AarhusSpaceProgramAPI.Models;

public class Manager
{
    public int ManagerId { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public DateTime HireDate { get; set; }
    
    public ICollection<Mission> Missions { get; set; } = new List<Mission>();
}