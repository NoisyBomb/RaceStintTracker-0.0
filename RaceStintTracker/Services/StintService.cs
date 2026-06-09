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
    public async Task RecalculateFromStint(int stintId)
    {
        var changedStint = await _context.Stints
            .Include(s => s.Race)
            .FirstOrDefaultAsync(s => s.Id == stintId);
    
        if (changedStint == null) throw new Exception("Stint not found");
    
        var race = changedStint.Race!;
    
        // Пересчитываем EndTime изменённого стинта
        changedStint.StintEndTime = changedStint.StintStartTime 
                                    + race.LapTime * changedStint.Laps 
                                    + race.PitTimeSpent;

        // Берём все последующие стинты этой гонки по порядку
        var nextStints = await _context.Stints
            .Where(s => s.RaceId == changedStint.RaceId 
                        && s.StintStartTime > changedStint.StintStartTime)
            .OrderBy(s => s.StintStartTime)
            .ToListAsync();

        TimeSpan currentTime = changedStint.StintEndTime;
        TimeSpan raceEnd = changedStint.StintStartTime - changedStint.StintStartTime 
                           + race.RaceDuration; // пересчитаем ниже

        // Находим время старта гонки — это StartTime первого стинта
        var firstStint = await _context.Stints
            .Where(s => s.RaceId == changedStint.RaceId)
            .OrderBy(s => s.StintStartTime)
            .FirstAsync();
    
        TimeSpan raceStart = firstStint.StintStartTime;
        raceEnd = raceStart + race.RaceDuration;

        foreach (var stint in nextStints)
        {
            stint.StintStartTime = currentTime;
            TimeSpan stintEnd = currentTime + race.LapTime * stint.Laps + race.PitTimeSpent;
            // Обрезаем последний стинт если выходит за финиш
            if (stintEnd > raceEnd)
                stintEnd = raceEnd;
            stint.StintEndTime = stintEnd;
            currentTime = stintEnd;
        }
        var lastStint = nextStints.LastOrDefault() ?? changedStint;
        if (lastStint.StintEndTime < raceEnd)
        {
            TimeSpan remaining = raceEnd - lastStint.StintStartTime - race.PitTimeSpent;
            int maxLaps = (int)(race.TankCapacity / race.FuelPerLap);
            int lapsToFinish = (int)(remaining / race.LapTime);
            lastStint.Laps = Math.Min(lapsToFinish, maxLaps);
            lastStint.StintEndTime = raceEnd;
        }
        await _context.SaveChangesAsync();
    }
}