using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceStintTracker.Data;
using RaceStintTracker.Models;

[ApiController]
[Route("[controller]")]

public class StintsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StintsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stints = await _context.Stints
            .Include(s => s.Driver)
            .Include(s => s.Race)
            .ToListAsync();
        return Ok(stints);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var stint = await _context.Stints
            .Include(s => s.Driver)
            .Include(s => s.Race)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (stint == null) return NotFound();
        return Ok(stint);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Stint stint)
    {
        var race = await _context.Races.FirstOrDefaultAsync(r => r.Id == stint.RaceId);
        if (race == null) return BadRequest("Race not found");
        var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == stint.DriverId);
        if (driver == null) return BadRequest("Driver not found");
        _context.Stints.Add(stint);
        await _context.SaveChangesAsync();
        return Created($"/stints/{stint.Id}", stint);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Stint stint)
    {
        var existing = await _context.Stints.FirstOrDefaultAsync(r => r.Id == id);
        if (existing == null) return NotFound();
        existing.DriverId = stint.DriverId;
        existing.Laps = stint.Laps;
        existing.StintStartTime = stint.StintStartTime;
        existing.StintEndTime = stint.StintEndTime;
        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var stint = await _context.Stints.FirstOrDefaultAsync(s => s.Id == id);
        if (stint == null) return NotFound();
        _context.Stints.Remove(stint);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}