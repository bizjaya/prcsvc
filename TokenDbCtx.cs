using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace PRCSVC;

public class TokenDbCtx : DbContext
{
    public DbSet<Token> Tokens { get; set; }

    public TokenDbCtx(DbContextOptions<TokenDbCtx> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Additional configuration here if needed
    }
}
