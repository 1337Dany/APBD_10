using APBD_10.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_10.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}