namespace RaceStintTracker.Models;

public class Driver
{
    public int Id { get; set; }
    public string DriverName { get; set; }
    
    public List<Stint> Stints { get; set; } = new();
}