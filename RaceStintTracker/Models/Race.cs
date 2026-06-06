namespace RaceStintTracker.Models;

public class Race
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Track { get; set; }
    public int TotalLaps { get; set; }
    public TimeSpan LapTime { get; set; }
    public double FuelPerLap { get; set; }
    public TimeSpan PitTimeSpent { get; set; }
    
    public List<Stint> Stints { get; set; } = new();
}