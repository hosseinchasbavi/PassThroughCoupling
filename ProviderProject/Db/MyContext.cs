using Microsoft.EntityFrameworkCore;
using ProviderProject.Entities;

namespace ProviderProject.Db;

public class MyContext : DbContext
{
    public MyContext(DbContextOptions<MyContext> options)
        : base(options)
    {
    }

    public DbSet<Request> Request { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Request>().Property(p => p.Id).IsRequired().UseIdentityColumn();
        modelBuilder.Entity<Request>().Property(p => p.Context).IsRequired().HasMaxLength(50);
    }
}