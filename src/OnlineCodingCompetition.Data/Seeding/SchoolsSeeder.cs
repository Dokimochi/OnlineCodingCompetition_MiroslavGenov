namespace OnlineCodingCompetition.Data.Seeding;

using Microsoft.EntityFrameworkCore;
using OnlineCodingCompetition.Data.Entities;

public class SchoolsSeeder : ISeeder
{
    public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        if (await dbContext.Schools.AnyAsync())
        {
            return;
        }

        await dbContext.Schools.AddRangeAsync(
            new School { Name = "Професионална гимназия по компютърни технологии", City = "София" },
            new School { Name = "Математическа гимназия", City = "Пловдив" },
            new School { Name = "Професионална гимназия по електроника", City = "Русе" });
    }
}
