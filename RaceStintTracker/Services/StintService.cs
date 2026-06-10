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
        // Если после пересчёта до финиша осталось время
        var lastStint = nextStints.LastOrDefault() ?? changedStint;
        TimeSpan timeAfterLast = raceEnd - lastStint.StintEndTime;
        int extraLaps = (int)(timeAfterLast / race.LapTime);
        if (extraLaps > 0)
        {
            int maxLaps = (int)(race.TankCapacity / race.FuelPerLap);
            if (lastStint.Laps < maxLaps)
            {
                int canAdd = maxLaps - lastStint.Laps;
                int toAdd = Math.Min(extraLaps, canAdd);
                lastStint.Laps += toAdd;
                lastStint.StintEndTime = lastStint.StintStartTime
                                         + race.LapTime * lastStint.Laps
                                         + race.PitTimeSpent;
                TimeSpan stillRemaining = raceEnd - lastStint.StintEndTime;
                int stillLaps = (int)(stillRemaining / race.LapTime);
                if (stillLaps > 0)
                {
                    var allStints = await _context.Stints
                        .Where(s => s.RaceId == changedStint.RaceId)
                        .OrderBy(s => s.StintStartTime)
                        .ToListAsync();
                    var driverIds = allStints.Select(s => s.DriverId).Distinct().ToList();
                    int lastDriverIndex = driverIds.IndexOf(lastStint.DriverId);
                    int nextDriverId = driverIds[(lastDriverIndex + 1) % driverIds.Count];
                    _context.Stints.Add(new Stint
                    {
                        RaceId = changedStint.RaceId,
                        DriverId = nextDriverId,
                        Laps = stillLaps,
                        StintStartTime = lastStint.StintEndTime,
                        StintEndTime = raceEnd
                    });
                }
            }
            else
            {
                // Последний стинт полный — добавляем новый
                var allStints = await _context.Stints
                    .Where(s => s.RaceId == changedStint.RaceId)
                    .OrderBy(s => s.StintStartTime)
                    .ToListAsync();
                var driverIds = allStints.Select(s => s.DriverId).Distinct().ToList();
                int lastDriverIndex = driverIds.IndexOf(lastStint.DriverId);
                int nextDriverId = driverIds[(lastDriverIndex + 1) % driverIds.Count];
                _context.Stints.Add(new Stint
                {
                    RaceId = changedStint.RaceId,
                    DriverId = nextDriverId,
                    Laps = extraLaps,
                    StintStartTime = lastStint.StintEndTime,
                    StintEndTime = raceEnd
                });
            }
        }
        await _context.SaveChangesAsync();
    }
}