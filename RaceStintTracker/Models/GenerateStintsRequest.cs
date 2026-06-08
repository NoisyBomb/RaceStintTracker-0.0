namespace RaceStintTracker.Models;

public class GenerateStintsRequest
{
    public int RaceId { get; set; }
    public TimeSpan RaceStart { get; set; }
    public List<int> DriverIds { get; set; } = new();
}