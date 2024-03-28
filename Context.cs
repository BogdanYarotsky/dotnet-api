using Microsoft.EntityFrameworkCore;

class Context : DbContext
{
    public DbSet<User> Users { get; init; }

    public Context(DbContextOptions<Context> options) :base(options)
    {
    }
}