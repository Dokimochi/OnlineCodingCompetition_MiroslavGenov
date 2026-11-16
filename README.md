# Платформа за онлайн състезания по програмиране

Дипломен проект на Мирослав Генов, ЧПГДН „СофтУни Будител“.

Уеб платформа, в която организаторите създават състезания и задачи с тестове, участниците изпращат решения, а системата ги проверява автоматично, изчислява точките и показва класиране.

## Технологии

- C# и ASP.NET Core MVC (.NET 8)
- MS SQL Server и Entity Framework Core 8 (Code First, миграции, seeding)
- ASP.NET Core Identity: роли Участник (`Contestant`), Организатор (`Organizer`), Администратор (`Administrator`)
- xUnit за тестовете, GitHub Actions за CI

## Структура

```
src/
  OnlineCodingCompetition.Data/       достъп до данни: Entities, DbContext, конфигурации, миграции, seeding
  OnlineCodingCompetition.Services/   бизнес логика
  OnlineCodingCompetition.Web/        потребителски интерфейс: MVC контролери, изгледи, модели
tests/
  OnlineCodingCompetition.Services.Tests/
docs/                                 документация
```

## Стартиране

Нужни са .NET 8 SDK и SQL Server (на Windows е достатъчен SQL Server LocalDB, който идва с Visual Studio).

```bash
dotnet tool restore
```

```bash
dotnet user-secrets set "Seeding:DemoPassword" "<парола за демо акаунтите>" --project src/OnlineCodingCompetition.Web
```

```bash
dotnet run --project src/OnlineCodingCompetition.Web
```

При стартиране приложението прилага миграциите и добавя демо данни (в режим Development). Демо акаунтите и останалите подробности са в [docs/02-baza-danni.md](docs/02-baza-danni.md).

За друг SQL Server задайте низа за връзка също с User Secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<низ за връзка>" --project src/OnlineCodingCompetition.Web
```

## Документация

- [Увод](docs/00-uvod.md)
- [Проучване на сходни приложения](docs/01-prouchvane.md)
- [База данни](docs/02-baza-danni.md)
