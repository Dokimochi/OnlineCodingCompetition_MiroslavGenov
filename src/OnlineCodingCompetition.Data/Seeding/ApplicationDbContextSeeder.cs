namespace OnlineCodingCompetition.Data.Seeding;

/// <summary>
/// Пуска всички seeders в правилния ред.
/// Ролите и езиците за програмиране не са тук: те идват от миграцията (HasData).
/// </summary>
public class ApplicationDbContextSeeder : ISeeder
{
    public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        var seeders = new ISeeder[]
        {
            new SchoolsSeeder(),
            new UsersSeeder(),
            new DemoContestsSeeder(),
        };

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync(dbContext, serviceProvider);
            await dbContext.SaveChangesAsync();
        }
    }
}
