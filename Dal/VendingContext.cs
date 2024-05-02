using Domain;
using Microsoft.EntityFrameworkCore;

namespace Dal;

public sealed class VendingContext : DbContext
{
    public VendingContext(DbContextOptions<VendingContext> options) : base(options) => Database.EnsureCreated();
    
    public DbSet<Drink> Drinks { get; set; } = null!;
    
    public DbSet<Coin> Coins { get; set; } = null!;
}