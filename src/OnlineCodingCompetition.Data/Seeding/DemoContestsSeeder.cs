namespace OnlineCodingCompetition.Data.Seeding;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineCodingCompetition.Data.Entities;
using OnlineCodingCompetition.Data.Enums;

/// <summary>
/// Две демо състезания: едно приключило (със задачи, решения и класиране)
/// и едно предстоящо с отворено записване. Датите се смятат спрямо момента на seeding.
/// </summary>
public class DemoContestsSeeder : ISeeder
{
    private const int CppId = 1;
    private const int CSharpId = 2;
    private const int JavaId = 3;
    private const int PythonId = 4;

    public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        if (await dbContext.Contests.AnyAsync())
        {
            return;
        }

        var users = await dbContext.Users.ToDictionaryAsync(u => u.Email!);
        if (!users.TryGetValue(DemoAccounts.OrganizerEmail, out var organizer))
        {
            serviceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger<DemoContestsSeeder>()
                .LogWarning("Демо състезанията не са създадени, защото няма демо организатор.");
            return;
        }

        var ivan = users[DemoAccounts.ContestantEmail];
        var elena = users[DemoAccounts.SecondContestantEmail];
        var petar = users[DemoAccounts.ThirdContestantEmail];

        var now = DateTime.UtcNow;
        var today = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0, DateTimeKind.Utc);

        // 1. Приключило състезание
        var autumnStart = today.AddDays(-10);
        var autumn = new Contest
        {
            Title = "Есенно състезание по програмиране",
            Description = "Две задачи за 4 часа. Всеки тест носи точки, класирането е по сбора от най-добрите решения.",
            RegistrationStart = today.AddDays(-40),
            RegistrationEnd = autumnStart.AddHours(-1),
            StartTime = autumnStart,
            EndTime = autumnStart.AddHours(4),
            ScoringType = ScoringType.Ioi,
            MinGrade = 8,
            MaxGrade = 12,
            IsPublished = true,
            Creator = organizer,
            CreatedOn = today.AddDays(-45),
        };

        var digitSum = new Problem
        {
            Title = "Сума на цифрите",
            Statement = "Дадено е цяло неотрицателно число N с до 100 цифри. Изведете сбора на цифрите му.\n\n" +
                        "**Вход:** един ред с числото N.\n\n**Изход:** едно цяло число.",
            OrderIndex = 1,
            TimeLimitMs = 1000,
            MemoryLimitMb = 256,
            CheckerType = CheckerType.Tokens,
            CreatedOn = autumn.CreatedOn,
        };
        AddTests(digitSum, ("12345", "15", 0, true), ("0", "0", 30, false), ("999999999", "81", 30, false),
            ("123456789012345678901234567890", "135", 40, false));

        var longestRun = new Problem
        {
            Title = "Най-дълга растяща серия",
            Statement = "Дадени са N цели числа. Намерете дължината на най-дългата поредица от съседни числа, " +
                        "в която всяко следващо е по-голямо от предишното.\n\n" +
                        "**Вход:** на първия ред N (1 ≤ N ≤ 100 000), на втория – N числа.\n\n**Изход:** едно цяло число.",
            OrderIndex = 2,
            TimeLimitMs = 1000,
            MemoryLimitMb = 256,
            CheckerType = CheckerType.Tokens,
            CreatedOn = autumn.CreatedOn,
        };
        AddTests(longestRun, ("5\n1 2 2 3 4", "3", 0, true), ("6\n1 2 3 4 5 6", "6", 30, false),
            ("4\n4 3 2 1", "1", 30, false), ("8\n1 3 2 4 5 6 1 2", "4", 40, false));

        autumn.Problems.Add(digitSum);
        autumn.Problems.Add(longestRun);
        foreach (var user in new[] { ivan, elena, petar })
        {
            autumn.Participants.Add(new ContestParticipant
            {
                User = user,
                Grade = user.Grade,
                RegisteredOn = autumn.RegistrationStart.AddDays(3),
            });
        }

        const TestVerdict ok = TestVerdict.Accepted;
        var submissions = new[]
        {
            // Иван: 100 + 100 = 200 точки
            Judged(digitSum, ivan, CppId, Code.CppDigitSum, autumnStart.AddMinutes(25), ok, ok, ok, ok),
            Judged(longestRun, ivan, PythonId, Code.PythonLongestRunBug, autumnStart.AddMinutes(70), ok, TestVerdict.WrongAnswer, ok, ok),
            Judged(longestRun, ivan, PythonId, Code.PythonLongestRun, autumnStart.AddMinutes(84), ok, ok, ok, ok),

            // Елена: 60 + 100 = 160 точки
            Judged(digitSum, elena, JavaId, Code.JavaDigitSumLong, autumnStart.AddMinutes(31), ok, ok, ok, TestVerdict.RuntimeError),
            Judged(longestRun, elena, CSharpId, Code.CSharpLongestRun, autumnStart.AddMinutes(102), ok, ok, ok, ok),

            // Петър: грешка при компилиране, после 100 точки
            Judged(digitSum, petar, CppId, Code.CppDigitSumNoSemicolon, autumnStart.AddMinutes(40)),
            Judged(digitSum, petar, CppId, Code.CppDigitSum, autumnStart.AddMinutes(47), ok, ok, ok, ok),
        };

        // 2. Предстоящо състезание с отворено записване
        var winterStart = today.AddDays(15);
        var winter = new Contest
        {
            Title = "Зимно състезание по програмиране",
            Description = "Записването е отворено. Задачите ще се видят в момента на старта.",
            RegistrationStart = today.AddDays(-2),
            RegistrationEnd = winterStart.AddDays(-1),
            StartTime = winterStart,
            EndTime = winterStart.AddHours(4),
            ScoringType = ScoringType.Ioi,
            MinGrade = 8,
            MaxGrade = 12,
            MaxParticipants = 200,
            IsPublished = true,
            Creator = organizer,
            CreatedOn = today.AddDays(-3),
        };

        var gridPaths = new Problem
        {
            Title = "Пътища в мрежа",
            Statement = "Робот е в горния ляв ъгъл на мрежа с R реда и C колони и може да се движи само надясно " +
                        "или надолу. По колко различни пътя може да стигне до долния десен ъгъл?\n\n" +
                        "**Вход:** R и C (1 ≤ R, C ≤ 15).\n\n**Изход:** броят на пътищата.",
            OrderIndex = 1,
            TimeLimitMs = 1000,
            MemoryLimitMb = 256,
            CheckerType = CheckerType.Tokens,
            CreatedOn = winter.CreatedOn,
        };
        AddTests(gridPaths, ("2 2", "2", 0, true), ("3 3", "6", 30, false), ("1 5", "1", 30, false), ("4 5", "35", 40, false));
        winter.Problems.Add(gridPaths);
        winter.Participants.Add(new ContestParticipant { User = ivan, Grade = ivan.Grade, RegisteredOn = today.AddDays(-1) });

        await dbContext.Contests.AddRangeAsync(autumn, winter);
        await dbContext.Submissions.AddRangeAsync(submissions);
        await dbContext.SaveChangesAsync();

        await dbContext.Notifications.AddRangeAsync(
            new Notification
            {
                User = ivan,
                Title = "Решението е проверено",
                Message = "Решението ви на „Най-дълга растяща серия“ получи 100 точки.",
                Link = $"/Contests/Details/{autumn.Id}",
                IsRead = true,
                CreatedOn = autumnStart.AddMinutes(84),
            },
            new Notification
            {
                User = ivan,
                Title = "Записахте се за състезание",
                Message = $"Записахте се за „{winter.Title}“. Стартът е на {winter.StartTime:dd.MM.yyyy} г.",
                Link = $"/Contests/Details/{winter.Id}",
                CreatedOn = today.AddDays(-1),
            },
            new Notification
            {
                User = elena,
                Title = "Класирането е публикувано",
                Message = $"Заехте 2-ро място в „{autumn.Title}“ със 160 точки.",
                Link = $"/Contests/Ranking/{autumn.Id}",
                CreatedOn = autumn.EndTime.AddHours(1),
            });
    }

    private static void AddTests(Problem problem, params (string Input, string Output, int Points, bool IsSample)[] tests)
    {
        for (var i = 0; i < tests.Length; i++)
        {
            problem.TestCases.Add(new TestCase
            {
                OrderIndex = i + 1,
                Input = tests[i].Input,
                ExpectedOutput = tests[i].Output,
                Points = tests[i].Points,
                IsSample = tests[i].IsSample,
            });
        }
    }

    /// <summary>
    /// Създава вече проверено решение. Без вердикти означава грешка при компилиране.
    /// Статусът е първият неуспешен вердикт, а точките са сборът от преминатите тестове.
    /// </summary>
    private static Submission Judged(
        Problem problem, ApplicationUser user, int languageId, string code, DateTime submittedOn, params TestVerdict[] verdicts)
    {
        var submission = new Submission
        {
            Problem = problem,
            User = user,
            ProgrammingLanguageId = languageId,
            SourceCode = code,
            SubmittedOn = submittedOn,
            JudgedOn = submittedOn.AddSeconds(4),
            Points = 0,
        };

        if (verdicts.Length == 0)
        {
            submission.Status = SubmissionStatus.CompilationError;
            submission.CompilerOutput = "main.cpp: In function 'int main()':\nmain.cpp:9:20: error: expected ';' before 'cout'";
            return submission;
        }

        var tests = problem.TestCases.OrderBy(t => t.OrderIndex).ToList();
        for (var i = 0; i < verdicts.Length; i++)
        {
            var passed = verdicts[i] == TestVerdict.Accepted;
            submission.TestResults.Add(new SubmissionTestResult
            {
                TestCase = tests[i],
                Verdict = verdicts[i],
                ExecutionTimeMs = 8 + (i * 5),
                MemoryUsedKb = languageId is JavaId or CSharpId ? 24_000 + (i * 300) : 3_200 + (i * 40),
                PointsAwarded = passed ? tests[i].Points : 0,
                CheckerMessage = passed ? null : verdicts[i] == TestVerdict.WrongAnswer
                    ? $"Очакван изход „{tests[i].ExpectedOutput}“, получен друг."
                    : "Програмата завърши с изключение.",
            });
        }

        var firstFailed = verdicts.FirstOrDefault(v => v != TestVerdict.Accepted);
        submission.Status = firstFailed switch
        {
            TestVerdict.WrongAnswer => SubmissionStatus.WrongAnswer,
            TestVerdict.TimeLimitExceeded => SubmissionStatus.TimeLimitExceeded,
            TestVerdict.MemoryLimitExceeded => SubmissionStatus.MemoryLimitExceeded,
            TestVerdict.RuntimeError => SubmissionStatus.RuntimeError,
            _ => SubmissionStatus.Accepted,
        };
        submission.Points = submission.TestResults.Sum(r => r.PointsAwarded);
        submission.ExecutionTimeMs = submission.TestResults.Max(r => r.ExecutionTimeMs);
        submission.MemoryUsedKb = submission.TestResults.Max(r => r.MemoryUsedKb);
        return submission;
    }

    /// <summary>Примерен код на демо решенията.</summary>
    private static class Code
    {
        public const string CppDigitSum =
            "#include <iostream>\n#include <string>\nusing namespace std;\n\nint main() {\n    string n;\n    cin >> n;\n" +
            "    int sum = 0;\n    for (char c : n) sum += c - '0';\n    cout << sum << endl;\n    return 0;\n}\n";

        public const string CppDigitSumNoSemicolon =
            "#include <iostream>\n#include <string>\nusing namespace std;\n\nint main() {\n    string n;\n    cin >> n;\n" +
            "    int sum = 0;\n    for (char c : n) sum += c - '0'\n    cout << sum << endl;\n    return 0;\n}\n";

        public const string PythonLongestRunBug =
            "n = int(input())\na = list(map(int, input().split()))\nbest = cur = 0\nfor i in range(1, n):\n" +
            "    cur = cur + 1 if a[i] > a[i - 1] else 1\n    best = max(best, cur)\nprint(best)\n";

        public const string PythonLongestRun =
            "n = int(input())\na = list(map(int, input().split()))\nbest = cur = 1\nfor i in range(1, n):\n" +
            "    cur = cur + 1 if a[i] > a[i - 1] else 1\n    best = max(best, cur)\nprint(best)\n";

        public const string JavaDigitSumLong =
            "import java.util.Scanner;\n\npublic class Main {\n    public static void main(String[] args) {\n" +
            "        long n = new Scanner(System.in).nextLong();\n        int sum = 0;\n" +
            "        while (n > 0) { sum += n % 10; n /= 10; }\n        System.out.println(sum);\n    }\n}\n";

        public const string CSharpLongestRun =
            "var n = int.Parse(Console.ReadLine()!);\nvar a = Console.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();\n" +
            "int best = 1, cur = 1;\nfor (var i = 1; i < n; i++)\n{\n    cur = a[i] > a[i - 1] ? cur + 1 : 1;\n    best = Math.Max(best, cur);\n}\n\nConsole.WriteLine(best);\n";
    }
}
