namespace RaceStintTracker.DTOs;

public class RaceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Track { get; set; } = string.Empty;
    public int TotalLaps { get; set; }
    public TimeSpan LapTime { get; set; }
    public double FuelPerLap { get; set; }
    public double TankCapacity { get; set; }
    public TimeSpan PitTimeSpent { get; set; }
    public TimeSpan RaceDuration { get; set; }
    public int StintCount { get; set; }
}