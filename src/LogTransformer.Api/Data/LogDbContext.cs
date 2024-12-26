using LogTransformer.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogTransformer.Api.Data;

public class LogDbContext : DbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options) : base(options) { }

    public DbSet<Log> Logs { get; set; }
}