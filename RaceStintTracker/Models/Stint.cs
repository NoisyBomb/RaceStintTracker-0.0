namespace RaceStintTracker.Models;

public class Stint
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public int DriverId { get; set; }
    public int Laps { get; set; }
    public TimeSpan StintStartTime { get; set; }
    public TimeSpan StintEndTime { get; set; }
    
    public Race Race { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
}