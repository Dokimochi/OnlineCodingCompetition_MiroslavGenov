namespace OnlineCodingCompetition.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineCodingCompetition.Data.Entities;

public class ProgrammingLanguageConfiguration : IEntityTypeConfiguration<ProgrammingLanguage>
{
    public void Configure(EntityTypeBuilder<ProgrammingLanguage> builder)
    {
        builder.HasIndex(l => l.Name).IsUnique();

        // Начални данни, които влизат в миграцията (HasData).
        builder.HasData(
            new ProgrammingLanguage { Id = 1, Name = "C++ 17", FileExtension = ".cpp", CompileCommand = "g++ -std=c++17 -O2 -o main main.cpp", RunCommand = "./main" },
            new ProgrammingLanguage { Id = 2, Name = "C# (.NET 8)", FileExtension = ".cs", CompileCommand = "dotnet build -c Release -o out", RunCommand = "dotnet out/Solution.dll" },
            new ProgrammingLanguage { Id = 3, Name = "Java 21", FileExtension = ".java", CompileCommand = "javac Main.java", RunCommand = "java Main" },
            new ProgrammingLanguage { Id = 4, Name = "Python 3", FileExtension = ".py", CompileCommand = null, RunCommand = "python3 main.py" },
            new ProgrammingLanguage { Id = 5, Name = "JavaScript (Node.js)", FileExtension = ".js", CompileCommand = null, RunCommand = "node main.js" });
    }
}
