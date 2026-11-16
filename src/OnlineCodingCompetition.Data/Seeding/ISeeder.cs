namespace OnlineCodingCompetition.Data.Seeding;

/// <summary>Добавя една група начални данни. Всеки seeder проверява дали данните вече съществуват.</summary>
public interface ISeeder
{
    Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
}
