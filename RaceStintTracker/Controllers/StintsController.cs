using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceStintTracker.Data;
using RaceStintTracker.Models;
using RaceStintTracker.Services;
using RaceStintTracker.DTOs;

namespace RaceStintTracker.Controllers;

[ApiController]
[Route("[controller]")]

public class StintsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly StintService _stintService;

    public StintsController(AppDbContext context, StintService stintService)
    {
        _stintService = stintService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stints = await _context.Stints
            .Include(s => s.Driver)
            .Include(s => s.Race)
            .OrderBy(s => s.RaceId)
            .ThenBy(s => s.StintStartTime)
            .ToListAsync();

        // Нумерация стинтов внутри каждой гонки отдельно
        var grouped = stints
            .GroupBy(s => s.RaceId)
            .SelectMany(g => g.Select((s, index) => new StintDto
            {
                Id = s.Id,
                StintNumber = index + 1,
                DriverName = s.Driver?.DriverName ?? "Unknown",
                Laps = s.Laps,
                StintStartTime = s.StintStartTime,
                StintEndTime = s.StintEndTime
            }))
            .ToList();
        return Ok(grouped);
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

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateStintsRequest request)
    {
        var stints = await _stintService.GenerateStints(
            request.RaceId,
            request.DriverIds,
            request.RaceStart);
        _context.Stints.AddRange(stints);
        await _context.SaveChangesAsync();
        return Ok(stints);
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