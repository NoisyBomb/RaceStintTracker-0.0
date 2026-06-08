namespace RaceStintTracker.DTOs;

public class StintDto
{
    public int Id { get; set; }
    public int StintNumber { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int Laps { get; set; }
    public TimeSpan StintStartTime { get; set; }
    public TimeSpan StintEndTime { get; set; }
}