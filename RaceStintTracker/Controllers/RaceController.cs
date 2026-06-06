using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceStintTracker.Data;
using RaceStintTracker.Models;



namespace RaceStintTracker.Controllers;

[ApiController]
[Route("[controller]")]

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
        return Ok(races);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var race = await _context.Races.Include(r => r.Stints).FirstOrDefaultAsync(r => r.Id == id);
        if (race == null) return NotFound();
        return Ok(race);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Race race)
    {
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