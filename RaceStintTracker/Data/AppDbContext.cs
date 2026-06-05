using Microsoft.EntityFrameworkCore; //библиотека для бд
using RaceStintTracker.Models; //библиотека с моделями из models

namespace RaceStintTracker.Data;

public class AppDbContext : DbContext //DbContext - класс из фрейма для общения с бд
{
    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options){ } //конструктор, настройки будут приходить из ependency injection
    public DbSet<Race> Races { get; set; } //представления таблицы в бд, передали Driver и будет таблица с колонками Id, DriverName
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Stint> Stints { get; set; }
}