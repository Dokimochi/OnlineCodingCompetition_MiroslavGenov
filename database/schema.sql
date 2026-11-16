-- =====================================================================
--  Онлайн платформа за състезания по програмиране
--  Начална версия на базата данни (Етап 1), PostgreSQL 16
--
--  Платформата поддържа два вида задачи:
--    * Auto – алгоритмични задачи, проверявани автоматично с тестове
--             (както в Codeforces, SoftUni Judge, CMS/IOI);
--    * Jury – проекти (приложение, сайт, игра), оценявани от жури
--             по критерии (както в „Аз мога – тук и сега“).
--
--  Изпълнение:  psql -d <база> -f database/schema.sql
-- =====================================================================

BEGIN;

-- ---------------------------------------------------------------------
-- 1. ПОТРЕБИТЕЛИ, РОЛИ, УЧИЛИЩА
-- ---------------------------------------------------------------------

-- Населени места (училищата са от много градове)
CREATE TABLE cities (
    id      SERIAL       PRIMARY KEY,
    name    VARCHAR(100) NOT NULL,
    region  VARCHAR(100) NOT NULL,                 -- област
    CONSTRAINT uq_cities_name_region UNIQUE (name, region)
);

-- Училища
CREATE TABLE schools (
    id            SERIAL       PRIMARY KEY,
    name          VARCHAR(200) NOT NULL,
    city_id       INT          NOT NULL REFERENCES cities(id),
    neispuo_code  VARCHAR(10)  UNIQUE,              -- код по НЕИСПУО
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- Роли: Administrator, Jury, Teacher, Contestant
CREATE TABLE roles (
    id    SMALLSERIAL PRIMARY KEY,
    name  VARCHAR(30) NOT NULL UNIQUE
);

-- Потребители (ученици, учители-ръководители, жури, администратори)
CREATE TABLE users (
    id                SERIAL       PRIMARY KEY,
    username          VARCHAR(50)  NOT NULL UNIQUE,
    email             VARCHAR(255) NOT NULL UNIQUE,
    password_hash     VARCHAR(255) NOT NULL,         -- само хеш, никога паролата
    first_name        VARCHAR(50)  NOT NULL,
    last_name         VARCHAR(50)  NOT NULL,
    github_username   VARCHAR(39),                   -- за предаване на проекти
    school_id         INT          REFERENCES schools(id),
    consent_given_at  TIMESTAMPTZ,                   -- съгласие по GDPR (родител при непълнолетни)
    is_active         BOOLEAN      NOT NULL DEFAULT TRUE,
    created_at        TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- Връзка много-към-много: един потребител може да има няколко роли
CREATE TABLE user_roles (
    user_id  INT      NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role_id  SMALLINT NOT NULL REFERENCES roles(id),
    PRIMARY KEY (user_id, role_id)
);

-- ---------------------------------------------------------------------
-- 2. СЪСТЕЗАНИЯ, НАПРАВЛЕНИЯ, ЖУРИ
-- ---------------------------------------------------------------------

-- Състезание = едно издание (напр. „XVI издание, 2027“)
CREATE TABLE contests (
    id                  SERIAL       PRIMARY KEY,
    title               VARCHAR(200) NOT NULL,
    edition             INT          CHECK (edition > 0),
    description         TEXT,
    location            VARCHAR(200),                -- град или „онлайн“
    registration_start  TIMESTAMPTZ  NOT NULL,
    registration_end    TIMESTAMPTZ  NOT NULL,
    start_at            TIMESTAMPTZ  NOT NULL,
    end_at              TIMESTAMPTZ  NOT NULL,
    status              VARCHAR(20)  NOT NULL DEFAULT 'Draft'
        CHECK (status IN ('Draft', 'Registration', 'Running', 'Grading', 'Finished')),
    created_by          INT          NOT NULL REFERENCES users(id),
    created_at          TIMESTAMPTZ  NOT NULL DEFAULT now(),
    CHECK (registration_start < registration_end),
    CHECK (registration_end  <= start_at),
    CHECK (start_at < end_at)
);

-- Каталог с направления (Програмиране, Front-end, Game Dev …)
CREATE TABLE categories (
    id           SMALLSERIAL  PRIMARY KEY,
    name         VARCHAR(100) NOT NULL UNIQUE,
    description  TEXT,
    is_active    BOOLEAN      NOT NULL DEFAULT TRUE
);

-- Направление в конкретно издание + възрастова група и правила
CREATE TABLE contest_categories (
    id             SERIAL      PRIMARY KEY,
    contest_id     INT         NOT NULL REFERENCES contests(id) ON DELETE CASCADE,
    category_id    SMALLINT    NOT NULL REFERENCES categories(id),
    min_grade      SMALLINT    NOT NULL CHECK (min_grade BETWEEN 1 AND 12),
    max_grade      SMALLINT    NOT NULL CHECK (max_grade BETWEEN 1 AND 12),
    max_team_size  SMALLINT    NOT NULL DEFAULT 1 CHECK (max_team_size BETWEEN 1 AND 5),
    max_teams      INT         CHECK (max_teams > 0),  -- лимит на местата (NULL = без лимит)
    start_at       TIMESTAMPTZ NOT NULL,               -- задачите стават видими чак тогава
    end_at         TIMESTAMPTZ NOT NULL,
    CHECK (min_grade <= max_grade),
    CHECK (start_at < end_at),
    CONSTRAINT uq_contest_category_group UNIQUE (contest_id, category_id, min_grade)
);

-- Жури, разпределено по направления
CREATE TABLE jury_assignments (
    contest_category_id  INT     NOT NULL REFERENCES contest_categories(id) ON DELETE CASCADE,
    user_id              INT     NOT NULL REFERENCES users(id),
    is_chair             BOOLEAN NOT NULL DEFAULT FALSE,   -- председател
    PRIMARY KEY (contest_category_id, user_id)
);

-- ---------------------------------------------------------------------
-- 3. ОТБОРИ И УЧАСТНИЦИ
--    Индивидуалното участие е „отбор от един човек“, затова решенията,
--    класирането и наградите винаги сочат към отбор.
-- ---------------------------------------------------------------------

CREATE TABLE teams (
    id                   SERIAL       PRIMARY KEY,
    contest_category_id  INT          NOT NULL REFERENCES contest_categories(id) ON DELETE CASCADE,
    name                 VARCHAR(100) NOT NULL,
    mentor_id            INT          REFERENCES users(id),   -- учител-ръководител
    status               VARCHAR(10)  NOT NULL DEFAULT 'Pending'
        CHECK (status IN ('Pending', 'Approved', 'Rejected', 'Withdrawn')),
    registered_at        TIMESTAMPTZ  NOT NULL DEFAULT now(),
    CONSTRAINT uq_team_name UNIQUE (contest_category_id, name)
);

CREATE TABLE team_members (
    team_id     INT      NOT NULL REFERENCES teams(id) ON DELETE CASCADE,
    user_id     INT      NOT NULL REFERENCES users(id),
    grade       SMALLINT NOT NULL CHECK (grade BETWEEN 1 AND 12), -- клас към момента на записване
    school_id   INT      REFERENCES schools(id),                  -- училище към момента на записване
    is_captain  BOOLEAN  NOT NULL DEFAULT FALSE,
    PRIMARY KEY (team_id, user_id)
);

-- Само един капитан в отбор
CREATE UNIQUE INDEX uq_team_one_captain ON team_members(team_id) WHERE is_captain;

-- ---------------------------------------------------------------------
-- 4. ЗАДАЧИ, ТЕСТОВЕ, КРИТЕРИИ, ЕЗИЦИ
-- ---------------------------------------------------------------------

CREATE TABLE programming_languages (
    id               SMALLSERIAL  PRIMARY KEY,
    name             VARCHAR(50)  NOT NULL UNIQUE,     -- „C++ 17 (g++)“
    file_extension   VARCHAR(10)  NOT NULL,
    compile_command  VARCHAR(300),                     -- NULL за интерпретируеми езици
    run_command      VARCHAR(300) NOT NULL,
    is_enabled       BOOLEAN      NOT NULL DEFAULT TRUE
);

CREATE TABLE problems (
    id                   SERIAL       PRIMARY KEY,
    contest_category_id  INT          REFERENCES contest_categories(id) ON DELETE SET NULL,
    title                VARCHAR(200) NOT NULL,
    statement            TEXT         NOT NULL,          -- условие (Markdown)
    evaluation_type      VARCHAR(10)  NOT NULL CHECK (evaluation_type IN ('Auto', 'Jury')),
    checker_type         VARCHAR(10)  CHECK (checker_type IN ('Exact', 'Tokens', 'Float')),
    max_points           NUMERIC(6,2) NOT NULL DEFAULT 100 CHECK (max_points > 0),
    time_limit_ms        INT          CHECK (time_limit_ms > 0),
    memory_limit_mb      INT          CHECK (memory_limit_mb > 0),
    order_index          SMALLINT     NOT NULL DEFAULT 1,
    is_public_archive    BOOLEAN      NOT NULL DEFAULT FALSE,  -- в архива за тренировка
    author_id            INT          REFERENCES users(id),
    created_at           TIMESTAMPTZ  NOT NULL DEFAULT now(),
    -- автоматично проверяваните задачи задължително имат лимити и чекър
    CHECK (evaluation_type = 'Jury'
           OR (time_limit_ms IS NOT NULL AND memory_limit_mb IS NOT NULL AND checker_type IS NOT NULL))
);

-- Тестове за автоматична проверка
CREATE TABLE test_cases (
    id               SERIAL       PRIMARY KEY,
    problem_id       INT          NOT NULL REFERENCES problems(id) ON DELETE CASCADE,
    order_index      SMALLINT     NOT NULL,
    input_data       TEXT         NOT NULL,
    expected_output  TEXT         NOT NULL,
    points           NUMERIC(6,2) NOT NULL DEFAULT 0 CHECK (points >= 0),
    is_sample        BOOLEAN      NOT NULL DEFAULT FALSE,   -- показва се в условието
    CONSTRAINT uq_test_order UNIQUE (problem_id, order_index)
);

-- Критерии за оценяване от жури (напр. „Коректност – 60 т.“)
CREATE TABLE evaluation_criteria (
    id           SERIAL       PRIMARY KEY,
    problem_id   INT          NOT NULL REFERENCES problems(id) ON DELETE CASCADE,
    name         VARCHAR(150) NOT NULL,
    description  TEXT,
    max_points   NUMERIC(6,2) NOT NULL CHECK (max_points > 0),
    order_index  SMALLINT     NOT NULL,
    CONSTRAINT uq_criterion_order UNIQUE (problem_id, order_index)
);

-- ---------------------------------------------------------------------
-- 5. РЕШЕНИЯ И ОЦЕНЯВАНЕ
-- ---------------------------------------------------------------------

CREATE TABLE submissions (
    id              BIGSERIAL    PRIMARY KEY,
    problem_id      INT          NOT NULL REFERENCES problems(id),
    team_id         INT          REFERENCES teams(id),            -- NULL = тренировка в архива
    user_id         INT          NOT NULL REFERENCES users(id),   -- кой член е предал
    language_id     SMALLINT     REFERENCES programming_languages(id),
    source_code     TEXT,                                         -- за Auto задачи
    repository_url  VARCHAR(300),                                 -- GitHub хранилище за Jury задачи
    commit_hash     CHAR(40),
    status          VARCHAR(20)  NOT NULL DEFAULT 'Queued'
        CHECK (status IN ('Queued', 'Compiling', 'Running',
                          'Accepted', 'PartiallyAccepted', 'WrongAnswer',
                          'TimeLimit', 'MemoryLimit', 'RuntimeError', 'CompileError',
                          'PendingReview', 'Reviewed', 'SystemError')),
    score           NUMERIC(6,2) CHECK (score >= 0),              -- точки при Auto задачи
    compile_output  TEXT,
    max_time_ms     INT,
    max_memory_kb   INT,
    submitted_at    TIMESTAMPTZ  NOT NULL DEFAULT now(),
    judged_at       TIMESTAMPTZ,
    CHECK (source_code IS NOT NULL OR repository_url IS NOT NULL),
    CHECK (source_code IS NULL OR language_id IS NOT NULL)
);

CREATE INDEX ix_submissions_team_problem ON submissions(team_id, problem_id);
CREATE INDEX ix_submissions_user         ON submissions(user_id);
CREATE INDEX ix_submissions_queue        ON submissions(submitted_at) WHERE status = 'Queued';

-- Резултат от всеки тест при автоматичната проверка
CREATE TABLE submission_test_results (
    submission_id    BIGINT       NOT NULL REFERENCES submissions(id) ON DELETE CASCADE,
    test_case_id     INT          NOT NULL REFERENCES test_cases(id) ON DELETE CASCADE,
    verdict          VARCHAR(20)  NOT NULL
        CHECK (verdict IN ('Accepted', 'WrongAnswer', 'TimeLimit', 'MemoryLimit', 'RuntimeError')),
    time_ms          INT,
    memory_kb        INT,
    points_awarded   NUMERIC(6,2) NOT NULL DEFAULT 0 CHECK (points_awarded >= 0),
    checker_message  VARCHAR(500),
    PRIMARY KEY (submission_id, test_case_id)
);

-- Точки от всеки член на журито по всеки критерий
CREATE TABLE jury_scores (
    id              SERIAL       PRIMARY KEY,
    submission_id   BIGINT       NOT NULL REFERENCES submissions(id) ON DELETE CASCADE,
    criterion_id    INT          NOT NULL REFERENCES evaluation_criteria(id) ON DELETE CASCADE,
    jury_member_id  INT          NOT NULL REFERENCES users(id),
    points          NUMERIC(6,2) NOT NULL CHECK (points >= 0),
    comment         TEXT,
    scored_at       TIMESTAMPTZ  NOT NULL DEFAULT now(),
    CONSTRAINT uq_jury_score UNIQUE (submission_id, criterion_id, jury_member_id)
);

-- ---------------------------------------------------------------------
-- 6. КОМУНИКАЦИЯ И РЕЗУЛТАТИ
-- ---------------------------------------------------------------------

-- Въпроси към журито по време на състезанието и обявления
CREATE TABLE clarifications (
    id                   SERIAL      PRIMARY KEY,
    contest_category_id  INT         NOT NULL REFERENCES contest_categories(id) ON DELETE CASCADE,
    problem_id           INT         REFERENCES problems(id),
    team_id              INT         REFERENCES teams(id),   -- NULL = обявление от журито
    question             TEXT,
    answer               TEXT,
    answered_by          INT         REFERENCES users(id),
    is_public            BOOLEAN     NOT NULL DEFAULT FALSE, -- видимо за всички отбори
    created_at           TIMESTAMPTZ NOT NULL DEFAULT now(),
    answered_at          TIMESTAMPTZ,
    CHECK (team_id IS NOT NULL OR is_public),                -- обявленията са публични
    CHECK (question IS NOT NULL OR answer IS NOT NULL)
);

-- Официално класиране след приключване (замразено копие + награди)
CREATE TABLE final_results (
    team_id           INT          PRIMARY KEY REFERENCES teams(id) ON DELETE CASCADE,
    total_score       NUMERIC(7,2) NOT NULL,
    rank              INT          NOT NULL CHECK (rank > 0),
    award             VARCHAR(20)  NOT NULL DEFAULT 'Participation'
        CHECK (award IN ('First', 'Second', 'Third', 'Honorable', 'Participation')),
    certificate_code  VARCHAR(30)  UNIQUE,                   -- номер на грамотата
    published_at      TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- ---------------------------------------------------------------------
-- 7. БИЗНЕС ПРАВИЛА (тригери)
-- ---------------------------------------------------------------------

-- При записване в отбор се проверява:
--   1) класът на ученика е в допустимата група;
--   2) отборът не е пълен;
--   3) ученикът участва само в едно направление на изданието.
CREATE FUNCTION fn_check_team_member() RETURNS trigger AS $$
DECLARE
    cc            contest_categories%ROWTYPE;
    members_count INT;
BEGIN
    SELECT c.* INTO cc
    FROM contest_categories c
    JOIN teams t ON t.contest_category_id = c.id
    WHERE t.id = NEW.team_id;

    IF NEW.grade NOT BETWEEN cc.min_grade AND cc.max_grade THEN
        RAISE EXCEPTION 'Ученик от % клас не може да участва в група % – % клас',
            NEW.grade, cc.min_grade, cc.max_grade;
    END IF;

    SELECT COUNT(*) INTO members_count
    FROM team_members
    WHERE team_id = NEW.team_id AND user_id <> NEW.user_id;

    IF members_count + 1 > cc.max_team_size THEN
        RAISE EXCEPTION 'Отборът е пълен (най-много % участници)', cc.max_team_size;
    END IF;

    IF EXISTS (
        SELECT 1
        FROM team_members tm
        JOIN teams t              ON t.id = tm.team_id
        JOIN contest_categories c ON c.id = t.contest_category_id
        WHERE tm.user_id = NEW.user_id
          AND c.contest_id = cc.contest_id
          AND tm.team_id <> NEW.team_id
    ) THEN
        RAISE EXCEPTION 'Потребител % вече участва в това издание', NEW.user_id;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_check_team_member
    BEFORE INSERT OR UPDATE ON team_members
    FOR EACH ROW EXECUTE FUNCTION fn_check_team_member();

-- Оценка от жури: точките не надвишават максимума на критерия
-- и оценяващият е разпределен като жури в направлението на задачата.
CREATE FUNCTION fn_check_jury_score() RETURNS trigger AS $$
DECLARE
    v_max      NUMERIC(6,2);
    v_category INT;
BEGIN
    SELECT ec.max_points, p.contest_category_id INTO v_max, v_category
    FROM evaluation_criteria ec
    JOIN problems p ON p.id = ec.problem_id
    WHERE ec.id = NEW.criterion_id;

    IF NEW.points > v_max THEN
        RAISE EXCEPTION 'Точките (%) надвишават максимума на критерия (%)', NEW.points, v_max;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM jury_assignments
        WHERE contest_category_id = v_category AND user_id = NEW.jury_member_id
    ) THEN
        RAISE EXCEPTION 'Потребител % не е жури в това направление', NEW.jury_member_id;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_check_jury_score
    BEFORE INSERT OR UPDATE ON jury_scores
    FOR EACH ROW EXECUTE FUNCTION fn_check_jury_score();

-- ---------------------------------------------------------------------
-- 8. ИЗГЛЕДИ (VIEWS) ЗА КЛАСИРАНЕ
-- ---------------------------------------------------------------------

-- Окончателни точки на всяко решение:
--   Auto – точките от тестовете;
--   Jury – сума по критерии от средната оценка на членовете на журито.
CREATE VIEW v_submission_scores AS
SELECT s.id      AS submission_id,
       s.team_id,
       s.problem_id,
       s.submitted_at,
       CAST(CASE WHEN p.evaluation_type = 'Auto' THEN s.score
                 ELSE j.jury_score
            END AS NUMERIC(7,2)) AS final_score
FROM submissions s
JOIN problems p ON p.id = s.problem_id
LEFT JOIN (
    SELECT submission_id, ROUND(SUM(avg_points), 2) AS jury_score
    FROM (
        SELECT submission_id, criterion_id, AVG(points) AS avg_points
        FROM jury_scores
        GROUP BY submission_id, criterion_id
    ) per_criterion
    GROUP BY submission_id
) j ON j.submission_id = s.id;

-- Текущо класиране: за всяка задача се взима най-доброто решение на отбора.
CREATE VIEW v_standings AS
WITH best AS (
    SELECT team_id, problem_id, MAX(final_score) AS best_score
    FROM v_submission_scores
    WHERE team_id IS NOT NULL AND final_score IS NOT NULL
    GROUP BY team_id, problem_id
)
SELECT t.contest_category_id,
       t.id                                  AS team_id,
       t.name                                AS team_name,
       COALESCE(SUM(b.best_score), 0)        AS total_score,
       COUNT(b.problem_id)                   AS problems_scored,
       RANK() OVER (PARTITION BY t.contest_category_id
                    ORDER BY COALESCE(SUM(b.best_score), 0) DESC) AS rank
FROM teams t
LEFT JOIN best b ON b.team_id = t.id
WHERE t.status = 'Approved'
GROUP BY t.contest_category_id, t.id, t.name;

COMMIT;
