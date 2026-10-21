using Microsoft.EntityFrameworkCore;
using SmartCurrencyAssistantWeb.Models;

namespace SmartCurrencyAssistantWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<CurrencyConversion> CurrencyConversions => Set<CurrencyConversion>();
    }
}