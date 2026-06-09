using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RaceStintTracker.Models;
using RaceStintTracker.Data;
using RaceStintTracker.DTOs;

namespace RaceStintTracker.Controllers;

[ApiController]
[Route("[controller]")]
public class DriversController : ControllerBase
{
    private readonly AppDbContext _context;  //ссылка на базу данных

    public DriversController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var drivers = await _context.Drivers.ToListAsync(); // _context.Drivers — это таблица Drivers в БД, ToListAsync() = SELECT * FROM Drivers
        return Ok(drivers);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Driver driver)
    {
        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();
        return Created($"/drivers/{driver.Id}", driver);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var driver = await _context.Drivers.Include(d => d.Stints).FirstOrDefaultAsync(d => d.Id == id);
        if (driver == null) return  NotFound();
        return Ok(driver);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Driver driver)
    {
        var existing = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == id);

        if (existing == null)
            return NotFound();

        existing.DriverName = driver.DriverName;
        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == id);

        if (driver == null)
            return NotFound();

        _context.Drivers.Remove(driver);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}