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
}