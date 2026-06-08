using RaceStintTracker.Data;
using RaceStintTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace RaceStintTracker.Services;

public class StintService
{
    private readonly AppDbContext _context;
    public StintService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Stint>> GenerateStints(int raceId, List<int> driverIds, TimeSpan raceStart)
    {
        var race = await _context.Races.FirstOrDefaultAsync(r => r.Id == raceId);
        if (race == null) throw new Exception("Race not found");
        int lapsPerStint = (int)(race.TankCapacity / race.FuelPerLap);
        TimeSpan raceEnd = raceStart + race.RaceDuration;

        var stints = new List<Stint>();
        TimeSpan currentTime = raceStart;
        int driverIndex = 0;
        int stintNummber = 1;

        while (currentTime < raceEnd)
        {
            TimeSpan stintDuration = race.LapTime * lapsPerStint;
            TimeSpan stintEnd = currentTime + stintDuration + race.PitTimeSpent;
            int laps = lapsPerStint;
            if (stintEnd > raceEnd)
            {
                stintEnd = raceEnd;
                TimeSpan remainingTime = raceEnd - currentTime;
                laps = (int)(remainingTime / race.LapTime);
            }
            stints.Add(new Stint
            {
                RaceId = raceId,
                DriverId = driverIds[driverIndex % driverIds.Count],
                Laps = laps,
                StintStartTime = currentTime,
                StintEndTime = stintEnd
            });
            currentTime = stintEnd;
            driverIndex++;
            stintNummber++;
        }
        return stints;
    }
}