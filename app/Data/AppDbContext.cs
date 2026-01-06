using Microsoft.EntityFrameworkCore;
using CatDiabetesLogger.Models;

namespace CatDiabetesLogger.Data;

public class AppDbContext : DbContext
{
    public DbSet<DailyLog> DailyLogs => Set<DailyLog>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
}
