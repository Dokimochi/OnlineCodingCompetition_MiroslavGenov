namespace OnlineCodingCompetition.Data.Seeding;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineCodingCompetition.Data.Common;
using OnlineCodingCompetition.Data.Entities;

/// <summary>Демо акаунти за трите роли. Паролата се взима от Seeding:DemoPassword.</summary>
public class UsersSeeder : ISeeder
{
    public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<SeedingOptions>>().Value;
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<UsersSeeder>();

        if (string.IsNullOrWhiteSpace(options.DemoPassword))
        {
            logger.LogWarning(
                "Демо акаунтите не са създадени: липсва настройка {Setting}. Задайте я с dotnet user-secrets.",
                $"{SeedingOptions.SectionName}:{nameof(SeedingOptions.DemoPassword)}");
            return;
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var schools = await dbContext.Schools.ToListAsync();

        var users = new (string Email, string FirstName, string LastName, byte? Grade, string? City, string Role)[]
        {
            (DemoAccounts.AdminEmail, "Мария", "Иванова", null, null, RoleNames.Administrator),
            (DemoAccounts.OrganizerEmail, "Георги", "Петров", null, "София", RoleNames.Organizer),
            (DemoAccounts.ContestantEmail, "Иван", "Димитров", 11, "София", RoleNames.Contestant),
            (DemoAccounts.SecondContestantEmail, "Елена", "Стоянова", 12, "Пловдив", RoleNames.Contestant),
            (DemoAccounts.ThirdContestantEmail, "Петър", "Николов", 10, "Русе", RoleNames.Contestant),
        };

        foreach (var data in users)
        {
            if (await userManager.FindByEmailAsync(data.Email) != null)
            {
                continue;
            }

            var user = new ApplicationUser
            {
                UserName = data.Email,
                Email = data.Email,
                EmailConfirmed = true,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Grade = data.Grade,
                SchoolId = schools.FirstOrDefault(s => s.City == data.City)?.Id,
                CreatedOn = DateTime.UtcNow,
            };

            var result = await userManager.CreateAsync(user, options.DemoPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Демо акаунтът {data.Email} не е създаден: " +
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            await userManager.AddToRoleAsync(user, data.Role);
        }
    }
}
