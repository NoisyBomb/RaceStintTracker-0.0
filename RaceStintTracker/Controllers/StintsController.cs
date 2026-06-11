using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceStintTracker.Data;
using RaceStintTracker.Models;
using RaceStintTracker.Services;
using RaceStintTracker.DTOs;

namespace RaceStintTracker.Controllers;

[ApiController]
[Route("api/[controller]")]

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
            .FirstOrDefaultAsync(s => s.Id == id);
        if (stint == null) return NotFound();
        var result = new StintDto
        {
            Id = stint.Id,
            StintNumber = 0, // без контекста всей гонки номер не считаем
            DriverName = stint.Driver?.DriverName ?? "Unknown",
            Laps = stint.Laps,
            StintStartTime = stint.StintStartTime,
            StintEndTime = stint.StintEndTime
        };
        return Ok(result);
    }
    
    [HttpGet("by-race/{raceId}")]
    public async Task<IActionResult> GetByRace(int raceId)
    {
        var stints = await _context.Stints
            .Include(s => s.Driver)
            .Where(s => s.RaceId == raceId)
            .OrderBy(s => s.StintStartTime)
            .ToListAsync();

        var result = stints.Select((s, index) => new StintDto
        {
            Id = s.Id,
            StintNumber = index + 1,
            DriverName = s.Driver?.DriverName ?? "Unknown",
            Laps = s.Laps,
            StintStartTime = s.StintStartTime,
            StintEndTime = s.StintEndTime
        }).ToList();

        return Ok(result);
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
        if (request.DriverIds == null || request.DriverIds.Count == 0)
            return BadRequest("DriverIds must be set");
        if (request.DriverIds.Count < 2)
            return BadRequest("At least 2 drivers are required");
        if (request.RaceStart <= TimeSpan.Zero)
            return BadRequest("RaceStart is required");
        var existingStints = await _context.Stints
            .Where(s => s.RaceId == request.RaceId)
            .AnyAsync();
        if (existingStints) return BadRequest("Stints already generated for this race");
        
        var stints = await _stintService.GenerateStints(
            request.RaceId,
            request.DriverIds,
            request.RaceStart);
        _context.Stints.AddRange(stints);
        await _context.SaveChangesAsync();

        // Загружаем пилотов для маппинга в DTO
        var driverIds = stints.Select(s => s.DriverId).Distinct().ToList();
        var drivers = await _context.Drivers
            .Where(d => driverIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.DriverName);

        var result = stints.Select((s, index) => new StintDto
        {
            Id = s.Id,
            StintNumber = index + 1,
            DriverName = drivers.GetValueOrDefault(s.DriverId, "Unknown"),
            Laps = s.Laps,
            StintStartTime = s.StintStartTime,
            StintEndTime = s.StintEndTime
        }).ToList();

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStint(int id, UpdateStintRequest request)
    {
        var existing = await _context.Stints
            .Include(s => s.Race)
            .FirstOrDefaultAsync(s => s.Id == id);
    
        if (existing == null) return NotFound();

        // Смена пилота
        if (request.DriverId.HasValue)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == request.DriverId.Value);
            if (driver == null) return BadRequest("Driver not found");
            existing.DriverId = request.DriverId.Value;
        }

        // Досрочный пит-стоп — уменьшение кругов
        if (request.Laps.HasValue)
        {
            if (request.Laps.Value > existing.Laps)
                return BadRequest("Laps can only be reduced (early pit stop)");
            if (request.Laps.Value <= 0)
                return BadRequest("Laps must be greater than 0");
        
            existing.Laps = request.Laps.Value;
            await _context.SaveChangesAsync();
            await _stintService.RecalculateFromStint(id);
            return Ok(existing);
        }
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