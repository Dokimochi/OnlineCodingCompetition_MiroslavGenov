namespace OnlineCodingCompetition.Web.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlineCodingCompetition.Data;
using OnlineCodingCompetition.Data.Seeding;

public static class DatabaseExtensions
{
    /// <summary>
    /// Прилага миграциите, които още не са в базата, и при Seeding:Enabled = true добавя демо данните.
    /// </summary>
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        var options = services.GetRequiredService<IOptions<SeedingOptions>>().Value;
        if (options.Enabled)
        {
            await new ApplicationDbContextSeeder().SeedAsync(dbContext, services);
        }
    }
}
