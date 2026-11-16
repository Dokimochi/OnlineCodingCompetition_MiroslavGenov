namespace OnlineCodingCompetition.Data.Common;

/// <summary>
/// Ограничения за дължината и стойностите на полетата.
/// Използват се и в Entities, и във ViewModels, за да съвпадат валидациите.
/// </summary>
public static class DataConstants
{
    public const int PersonNameMaxLength = 50;
    public const int SchoolNameMaxLength = 150;
    public const int CityMaxLength = 80;

    public const int TitleMaxLength = 100;
    public const int DescriptionMaxLength = 2000;

    public const int LanguageNameMaxLength = 50;
    public const int FileExtensionMaxLength = 10;
    public const int CommandMaxLength = 300;

    public const int SourceCodeMaxLength = 65536;
    public const int CheckerMessageMaxLength = 500;

    public const int NotificationTitleMaxLength = 100;
    public const int NotificationMessageMaxLength = 1000;
    public const int LinkMaxLength = 300;

    public const byte MinGrade = 1;
    public const byte MaxGrade = 12;
}
