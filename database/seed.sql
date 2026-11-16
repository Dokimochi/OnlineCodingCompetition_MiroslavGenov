-- =====================================================================
--  Демо данни за проверка на схемата (имената са измислени).
--  Изпълнение след schema.sql:  psql -d <база> -f database/seed.sql
-- =====================================================================

BEGIN;

-- Населени места и училища ---------------------------------------------
INSERT INTO cities (id, name, region) VALUES
    (1, 'Девин',   'Смолян'),
    (2, 'София',   'София-град'),
    (3, 'Пловдив', 'Пловдив'),
    (4, 'Русе',    'Русе');

INSERT INTO schools (id, name, city_id) VALUES
    (1, 'Професионална гимназия по компютърни технологии', 2),
    (2, 'Математическа гимназия',                          3),
    (3, 'Професионална гимназия по електроника',           4);

-- Роли и потребители (паролите са примерни хешове) -------------------------
INSERT INTO roles (id, name) VALUES
    (1, 'Administrator'), (2, 'Jury'), (3, 'Teacher'), (4, 'Contestant');

INSERT INTO users (id, username, email, password_hash, first_name, last_name, github_username, school_id, consent_given_at) VALUES
    ( 1, 'admin',    'admin@example.com',    '$2a$11$demoHashAdmin',   'Системен',   'Администратор', NULL,          NULL, now()),
    ( 2, 'mkoleva',  'mkoleva@example.com',  '$2a$11$demoHashJury1',   'Мария',      'Колева',        'mkoleva',     NULL, now()),
    ( 3, 'pdimitrov','pdimitrov@example.com','$2a$11$demoHashJury2',   'Петър',      'Димитров',      'pdimitrov',   NULL, now()),
    ( 4, 'eivanova', 'eivanova@example.com', '$2a$11$demoHashTeach1',  'Елена',      'Иванова',       NULL,          1,    now()),
    ( 5, 'gstoyanov','gstoyanov@example.com','$2a$11$demoHashTeach2',  'Георги',     'Стоянов',       NULL,          2,    now()),
    ( 6, 'ivan',     'ivan@example.com',     '$2a$11$demoHashStud1',   'Иван',       'Петров',        'ivan-p',      1,    now()),
    ( 7, 'nikol',    'nikol@example.com',    '$2a$11$demoHashStud2',   'Николета',   'Георгиева',     'nikol-g',     1,    now()),
    ( 8, 'dimitar',  'dimitar@example.com',  '$2a$11$demoHashStud3',   'Димитър',    'Ангелов',       'dangelov',    2,    now()),
    ( 9, 'viktoria', 'viktoria@example.com', '$2a$11$demoHashStud4',   'Виктория',   'Маринова',      'vmarinova',   3,    now()),
    (10, 'alex',     'alex@example.com',     '$2a$11$demoHashStud5',   'Александър', 'Христов',       'alex-h',      2,    now()),
    (11, 'yoana',    'yoana@example.com',    '$2a$11$demoHashStud6',   'Йоана',      'Тодорова',      'yoana-t',     3,    now());

INSERT INTO user_roles (user_id, role_id) VALUES
    (1, 1), (2, 2), (3, 2), (4, 3), (5, 3),
    (6, 4), (7, 4), (8, 4), (9, 4), (10, 4), (11, 4);

-- Състезание, направления, жури ----------------------------------------------
INSERT INTO contests (id, title, edition, description, location,
                      registration_start, registration_end, start_at, end_at, status, created_by) VALUES
    (1, 'Национално онлайн състезание по програмиране', 1,
        'Демо издание с алгоритмични задачи и проект, оценяван от жури.', 'Онлайн',
        '2026-03-01 00:00+02', '2026-04-10 23:59+03',
        '2026-04-25 09:00+03', '2026-04-25 17:00+03', 'Finished', 1);

INSERT INTO categories (id, name, description) VALUES
    (1, 'Алгоритмично програмиране',      'Задачи с автоматична проверка чрез тестове'),
    (2, 'Програмиране – приложение',      'Разработка на приложение по задание, оценява се от жури'),
    (3, 'Front-end web development',      'Изработка на уеб интерфейс по макет'),
    (4, 'Концепция и дизайн на видеоигри','Игра, разработена по време на състезанието');

INSERT INTO contest_categories (id, contest_id, category_id, min_grade, max_grade, max_team_size, start_at, end_at) VALUES
    (1, 1, 1,  7, 12, 1, '2026-04-25 09:00+03', '2026-04-25 13:00+03'),  -- индивидуално
    (2, 1, 2,  9, 12, 3, '2026-04-25 09:00+03', '2026-04-25 17:00+03');  -- отбори до 3-ма

INSERT INTO jury_assignments (contest_category_id, user_id, is_chair) VALUES
    (1, 2, TRUE),
    (2, 2, TRUE),
    (2, 3, FALSE);

-- Отбори и участници -----------------------------------------------------------
INSERT INTO teams (id, contest_category_id, name, mentor_id, status) VALUES
    (1, 1, 'Иван Петров',       4,    'Approved'),
    (2, 1, 'Димитър Ангелов',   5,    'Approved'),
    (3, 1, 'Виктория Маринова', NULL, 'Approved'),
    (4, 2, 'Byte Force',        4,    'Approved'),
    (5, 2, 'Null Pointers',     5,    'Approved');

INSERT INTO team_members (team_id, user_id, grade, school_id, is_captain) VALUES
    (1,  6, 10, 1, TRUE),
    (2,  8,  8, 2, TRUE),
    (3,  9, 11, 3, TRUE),
    (4,  7, 11, 1, TRUE),
    (4, 10, 12, 2, FALSE),
    (5, 11,  9, 3, TRUE);

-- Езици ------------------------------------------------------------------------
INSERT INTO programming_languages (id, name, file_extension, compile_command, run_command) VALUES
    (1, 'C++ 17 (g++)',    '.cpp',  'g++ -std=c++17 -O2 -o main main.cpp', './main'),
    (2, 'C# (.NET 8)',     '.cs',   'dotnet build -c Release',             'dotnet run --no-build -c Release'),
    (3, 'Java 21',         '.java', 'javac Main.java',                     'java Main'),
    (4, 'Python 3.12',     '.py',   NULL,                                  'python3 main.py'),
    (5, 'JavaScript (Node.js 22)', '.js', NULL,                            'node main.js');

-- Задачи, тестове, критерии -------------------------------------------------------
INSERT INTO problems (id, contest_category_id, title, statement, evaluation_type, checker_type,
                      max_points, time_limit_ms, memory_limit_mb, order_index, is_public_archive, author_id) VALUES
    (1, 1, 'Най-дълга серия',
        'Дадена е редица от N цели числа. Намерете дължината на най-дългата растяща серия.',
        'Auto', 'Tokens', 100, 1000, 256, 1, TRUE, 2),
    (2, 1, 'Пътища в града',
        'Даден е граф с N кръстовища и M улици. Намерете броя на най-кратките пътища от 1 до N.',
        'Auto', 'Exact', 100, 2000, 256, 2, TRUE, 2),
    (3, 2, 'Система за библиотека',
        'Създайте приложение за управление на книги, читатели и заемания.',
        'Jury', NULL, 100, NULL, NULL, 1, FALSE, 3);

INSERT INTO test_cases (id, problem_id, order_index, input_data, expected_output, points, is_sample) VALUES
    (1, 1, 1, E'5\n1 2 2 3 4',      '3', 0,  TRUE),
    (2, 1, 2, E'6\n1 2 3 4 5 6',    '6', 30, FALSE),
    (3, 1, 3, E'4\n4 3 2 1',        '1', 30, FALSE),
    (4, 1, 4, E'8\n1 3 2 4 5 6 1 2', '4', 40, FALSE),
    (5, 2, 1, E'3 3\n1 2\n2 3\n1 3', '1', 0,  TRUE),
    (6, 2, 2, E'4 4\n1 2\n1 3\n2 4\n3 4', '2', 50, FALSE),
    (7, 2, 3, E'5 6\n1 2\n1 3\n2 4\n3 4\n4 5\n2 5', '1', 50, FALSE);

INSERT INTO evaluation_criteria (id, problem_id, name, description, max_points, order_index) VALUES
    (1, 3, 'Коректност на имплементацията', 'Приложението изпълнява изискванията от заданието', 60, 1),
    (2, 3, 'Качество на кода',              'Структура, именуване, без повторения',            20, 2),
    (3, 3, 'Потребителски интерфейс',       'Удобство и прегледност',                          10, 3),
    (4, 3, 'Представяне',                   'Защита на проекта пред журито',                   10, 4);

-- Решения ------------------------------------------------------------------------
INSERT INTO submissions (id, problem_id, team_id, user_id, language_id, source_code, repository_url, commit_hash,
                         status, score, max_time_ms, max_memory_kb, submitted_at, judged_at) VALUES
    ( 1, 1, 1,  6, 1, '// C++ решение v1', NULL, NULL, 'WrongAnswer',  60, 12, 3200, '2026-04-25 09:41+03', '2026-04-25 09:41+03'),
    ( 2, 1, 1,  6, 1, '// C++ решение v2', NULL, NULL, 'Accepted',    100, 11, 3150, '2026-04-25 10:05+03', '2026-04-25 10:05+03'),
    ( 3, 2, 1,  6, 1, '// C++ решение',    NULL, NULL, 'WrongAnswer',  50, 35, 4100, '2026-04-25 12:10+03', '2026-04-25 12:10+03'),
    ( 4, 1, 2,  8, 4, '# Python решение',  NULL, NULL, 'Accepted',    100, 48, 9800, '2026-04-25 09:58+03', '2026-04-25 09:58+03'),
    ( 5, 2, 2,  8, 4, '# Python v1',       NULL, NULL, 'TimeLimit',    50, 2000, 12000, '2026-04-25 11:20+03', '2026-04-25 11:20+03'),
    ( 6, 2, 2,  8, 4, '# Python v2',       NULL, NULL, 'Accepted',    100, 640, 11800, '2026-04-25 12:02+03', '2026-04-25 12:02+03'),
    ( 7, 1, 3,  9, 3, '// Java v1',        NULL, NULL, 'CompileError',  0, NULL, NULL, '2026-04-25 10:15+03', '2026-04-25 10:15+03'),
    ( 8, 1, 3,  9, 3, '// Java v2',        NULL, NULL, 'WrongAnswer',  60, 90, 41000, '2026-04-25 10:30+03', '2026-04-25 10:30+03'),
    ( 9, 1, NULL, 7, 2, '// C# тренировка', NULL, NULL, 'Accepted',   100, 70, 28000, '2026-05-02 18:00+03', '2026-05-02 18:00+03'),
    (10, 3, 4,  7, NULL, NULL, 'https://github.com/byteforce-demo/library-system', 'a3f5c2e9b1d4f6a8c0e2b4d6f8a0c2e4b6d8f0a2',
         'Reviewed', NULL, NULL, NULL, '2026-04-25 16:55+03', NULL),
    (11, 3, 5, 11, NULL, NULL, 'https://github.com/nullpointers-demo/library', 'b4e6d3f0c2e5a7b9d1f3c5e7a9b1d3f5c7e9a1b3',
         'Reviewed', NULL, NULL, NULL, '2026-04-25 16:58+03', NULL);

-- Резултати по тестове (за част от решенията)
INSERT INTO submission_test_results (submission_id, test_case_id, verdict, time_ms, memory_kb, points_awarded) VALUES
    (1, 1, 'Accepted',    5, 3100,  0),
    (1, 2, 'Accepted',    8, 3150, 30),
    (1, 3, 'Accepted',    7, 3120, 30),
    (1, 4, 'WrongAnswer',12, 3200,  0),
    (2, 1, 'Accepted',    5, 3100,  0),
    (2, 2, 'Accepted',    8, 3150, 30),
    (2, 3, 'Accepted',    7, 3120, 30),
    (2, 4, 'Accepted',   11, 3150, 40),
    (5, 5, 'Accepted',   40, 9000,  0),
    (5, 6, 'Accepted',  120, 9500, 50),
    (5, 7, 'TimeLimit', 2000,12000, 0);

-- Оценки на журито (двама членове × 4 критерия)
INSERT INTO jury_scores (submission_id, criterion_id, jury_member_id, points, comment) VALUES
    (10, 1, 2, 55, 'Всички основни функции работят'),
    (10, 2, 2, 18, NULL),
    (10, 3, 2,  9, NULL),
    (10, 4, 2,  8, NULL),
    (10, 1, 3, 50, 'Липсва търсене по автор'),
    (10, 2, 3, 16, NULL),
    (10, 3, 3, 10, NULL),
    (10, 4, 3,  9, NULL),
    (11, 1, 2, 45, NULL),
    (11, 2, 2, 15, NULL),
    (11, 3, 2,  7, NULL),
    (11, 4, 2,  9, NULL),
    (11, 1, 3, 48, NULL),
    (11, 2, 3, 14, NULL),
    (11, 3, 3,  8, NULL),
    (11, 4, 3,  8, NULL);

-- Въпроси и обявления -------------------------------------------------------------
INSERT INTO clarifications (contest_category_id, problem_id, team_id, question, answer, answered_by, is_public, created_at, answered_at) VALUES
    (1, 1, 1,    'Може ли N да е 0?', 'Не, 1 ≤ N ≤ 100 000. Условието е допълнено.', 2, TRUE,
        '2026-04-25 09:20+03', '2026-04-25 09:25+03'),
    (1, NULL, NULL, NULL, 'Остават 30 минути до края на състезанието.', 2, TRUE,
        '2026-04-25 12:30+03', '2026-04-25 12:30+03');

-- Официално класиране: замразяваме текущото класиране и раздаваме награди
INSERT INTO final_results (team_id, total_score, rank, award, certificate_code)
SELECT team_id,
       total_score,
       rank,
       CASE rank WHEN 1 THEN 'First' WHEN 2 THEN 'Second' WHEN 3 THEN 'Third' ELSE 'Participation' END,
       format('OCC-2026-%s-%s', contest_category_id, lpad(rank::text, 3, '0'))
FROM v_standings;

-- Синхронизираме броячите на SERIAL колоните след ръчно зададените id
DO $$
DECLARE r record;
BEGIN
    FOR r IN
        SELECT table_name FROM information_schema.columns
        WHERE table_schema = 'public' AND column_name = 'id' AND column_default LIKE 'nextval%'
    LOOP
        EXECUTE format('SELECT setval(pg_get_serial_sequence(%L, ''id''), COALESCE((SELECT MAX(id) FROM %I), 1))',
                       r.table_name, r.table_name);
    END LOOP;
END $$;

COMMIT;
