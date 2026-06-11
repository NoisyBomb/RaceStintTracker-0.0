using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceStintTracker.Data;
using RaceStintTracker.Models;
using RaceStintTracker.DTOs;



namespace RaceStintTracker.Controllers;

[ApiController]
[Route("api/[controller]")]

public class RacesController : ControllerBase
{
    private readonly AppDbContext _context;
    public RacesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var races = await _context.Races.Include(r => r.Stints).ToListAsync();
        var result = races.Select(r => new RaceDto
        {
            Id = r.Id,
            Name = r.Name,
            Track = r.Track,
            TotalLaps = r.TotalLaps,
            LapTime = r.LapTime,
            FuelPerLap = r.FuelPerLap,
            TankCapacity = r.TankCapacity,
            PitTimeSpent = r.PitTimeSpent,
            RaceDuration = r.RaceDuration,
            StintCount = r.Stints?.Count ?? 0
        }).ToList();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var race = await _context.Races.Include(r => r.Stints).FirstOrDefaultAsync(r => r.Id == id);
        if (race == null) return NotFound();
        var result = new RaceDto
        {
            Id = race.Id,
            Name = race.Name,
            Track = race.Track,
            TotalLaps = race.TotalLaps,
            LapTime = race.LapTime,
            FuelPerLap = race.FuelPerLap,
            TankCapacity = race.TankCapacity,
            PitTimeSpent = race.PitTimeSpent,
            RaceDuration = race.RaceDuration,
            StintCount = race.Stints?.Count ?? 0
        };
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Race race)
    {
        if (string.IsNullOrEmpty(race.Name))
            return BadRequest("Name is required");
        if (string.IsNullOrEmpty(race.Track))
            return BadRequest("Track is required");
        if (race.FuelPerLap <= 0)
            return BadRequest("FuelPerLap must be greater than zero");
        if (race.TankCapacity <= 0)
            return BadRequest("TankCapacity must be greater than zero");
        if(race.LapTime <= TimeSpan.Zero)
            return BadRequest("LapTime must be greater than zero");
        if (race.PitTimeSpent <= TimeSpan.Zero)
            return BadRequest("PitTimeSpent must be greater than zero");
        if (race.RaceDuration <= TimeSpan.Zero)
            return BadRequest("RaceDuration must be greater than zero");
        _context.Races.Add(race);
        await _context.SaveChangesAsync();
        return Created("$/races{race.Id}", race);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Race race)
    {
        var existing = await _context.Races.FirstOrDefaultAsync(r => r.Id == id);
        if (existing == null) return NotFound();
        existing.Name = race.Name;
        existing.Track = race.Track;
        existing.FuelPerLap = race.FuelPerLap;
        existing.LapTime = race.LapTime;
        existing.PitTimeSpent = race.PitTimeSpent;
        existing.TotalLaps = race.TotalLaps;
        existing.TankCapacity = race.TankCapacity;
        existing.RaceDuration = race.RaceDuration;
        
        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var race = await _context.Races.FirstOrDefaultAsync(r => r.Id == id);
        if (race == null) return NotFound();
        _context.Races.Remove(race);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}