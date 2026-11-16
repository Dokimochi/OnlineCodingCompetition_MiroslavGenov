# База данни

Начална версия на базата данни (Етап 1). СУБД: **PostgreSQL 16**.

| Файл | Съдържание |
|------|------------|
| [`schema.sql`](schema.sql) | 20 таблици, ограничения, 2 тригера и 2 изгледа |
| [`seed.sql`](seed.sql) | Демо данни: 1 издание, 2 направления, 5 отбора, 11 решения |

## Пускане

```bash
createdb competition
psql -d competition -f database/schema.sql
psql -d competition -f database/seed.sql
psql -d competition -c "SELECT * FROM v_standings ORDER BY contest_category_id, rank;"
```

## Групи таблици

| Група | Таблици |
|-------|---------|
| Хора и училища | `users`, `roles`, `user_roles`, `schools`, `cities` |
| Състезания и направления | `contests`, `categories`, `contest_categories`, `jury_assignments` |
| Участие и резултати | `teams`, `team_members`, `clarifications`, `final_results` |
| Задачи | `problems`, `test_cases`, `evaluation_criteria`, `programming_languages` |
| Решения и оценяване | `submissions`, `submission_test_results`, `jury_scores` |

## Основни решения

1. **Два вида задачи.** `problems.evaluation_type` е `Auto` (проверка с тестове) или `Jury` (оценка от жури по критерии).
2. **Индивидуалното участие е отбор от един човек.** Решенията, класирането и наградите винаги сочат към `teams`.
3. **Издание → направление → възрастова група.** `contest_categories` пази класовете, размера на отбора и часа на провеждане.
4. **Класът и училището се пазят към момента на записване** в `team_members`.
5. **Класирането на живо е изглед** (`v_standings`), а официалното е замразено в `final_results`.
6. **Правилата от регламента са в базата** чрез тригерите `trg_check_team_member` и `trg_check_jury_score`.

## ER диаграма

```mermaid
erDiagram
    contest_categories ||--o{ clarifications : "contest_category_id"
    problems ||--o{ clarifications : "problem_id"
    teams ||--o{ clarifications : "team_id"
    users ||--o{ clarifications : "answered_by"
    categories ||--o{ contest_categories : "category_id"
    contests ||--o{ contest_categories : "contest_id"
    users ||--o{ contests : "created_by"
    problems ||--o{ evaluation_criteria : "problem_id"
    teams ||--o{ final_results : "team_id"
    contest_categories ||--o{ jury_assignments : "contest_category_id"
    users ||--o{ jury_assignments : "user_id"
    evaluation_criteria ||--o{ jury_scores : "criterion_id"
    submissions ||--o{ jury_scores : "submission_id"
    users ||--o{ jury_scores : "jury_member_id"
    contest_categories ||--o{ problems : "contest_category_id"
    users ||--o{ problems : "author_id"
    cities ||--o{ schools : "city_id"
    submissions ||--o{ submission_test_results : "submission_id"
    test_cases ||--o{ submission_test_results : "test_case_id"
    problems ||--o{ submissions : "problem_id"
    programming_languages ||--o{ submissions : "language_id"
    teams ||--o{ submissions : "team_id"
    users ||--o{ submissions : "user_id"
    schools ||--o{ team_members : "school_id"
    teams ||--o{ team_members : "team_id"
    users ||--o{ team_members : "user_id"
    contest_categories ||--o{ teams : "contest_category_id"
    users ||--o{ teams : "mentor_id"
    problems ||--o{ test_cases : "problem_id"
    roles ||--o{ user_roles : "role_id"
    users ||--o{ user_roles : "user_id"
    schools ||--o{ users : "school_id"
    cities {
        SERIAL id PK
        VARCHAR name
        VARCHAR region
    }
    schools {
        SERIAL id PK
        VARCHAR name
        INT city_id FK
        VARCHAR neispuo_code UK
        TIMESTAMPTZ created_at
    }
    roles {
        SMALLSERIAL id PK
        VARCHAR name UK
    }
    users {
        SERIAL id PK
        VARCHAR username UK
        VARCHAR email UK
        VARCHAR password_hash
        VARCHAR first_name
        VARCHAR last_name
        VARCHAR github_username
        INT school_id FK
        TIMESTAMPTZ consent_given_at
        BOOLEAN is_active
        TIMESTAMPTZ created_at
    }
    user_roles {
        INT user_id PK
        SMALLINT role_id PK
    }
    contests {
        SERIAL id PK
        VARCHAR title
        INT edition
        TEXT description
        VARCHAR location
        TIMESTAMPTZ registration_start
        TIMESTAMPTZ registration_end
        TIMESTAMPTZ start_at
        TIMESTAMPTZ end_at
        VARCHAR status
        INT created_by FK
        TIMESTAMPTZ created_at
    }
    categories {
        SMALLSERIAL id PK
        VARCHAR name UK
        TEXT description
        BOOLEAN is_active
    }
    contest_categories {
        SERIAL id PK
        INT contest_id FK
        SMALLINT category_id FK
        SMALLINT min_grade
        SMALLINT max_grade
        SMALLINT max_team_size
        INT max_teams
        TIMESTAMPTZ start_at
        TIMESTAMPTZ end_at
    }
    jury_assignments {
        INT contest_category_id PK
        INT user_id PK
        BOOLEAN is_chair
    }
    teams {
        SERIAL id PK
        INT contest_category_id FK
        VARCHAR name
        INT mentor_id FK
        VARCHAR status
        TIMESTAMPTZ registered_at
    }
    team_members {
        INT team_id PK
        INT user_id PK
        SMALLINT grade
        INT school_id FK
        BOOLEAN is_captain
    }
    clarifications {
        SERIAL id PK
        INT contest_category_id FK
        INT problem_id FK
        INT team_id FK
        TEXT question
        TEXT answer
        INT answered_by FK
        BOOLEAN is_public
        TIMESTAMPTZ created_at
        TIMESTAMPTZ answered_at
    }
    final_results {
        INT team_id PK
        NUMERIC total_score
        INT rank
        VARCHAR award
        VARCHAR certificate_code UK
        TIMESTAMPTZ published_at
    }
    problems {
        SERIAL id PK
        INT contest_category_id FK
        VARCHAR title
        TEXT statement
        VARCHAR evaluation_type
        VARCHAR checker_type
        NUMERIC max_points
        INT time_limit_ms
        INT memory_limit_mb
        SMALLINT order_index
        BOOLEAN is_public_archive
        INT author_id FK
        TIMESTAMPTZ created_at
    }
    test_cases {
        SERIAL id PK
        INT problem_id FK
        SMALLINT order_index
        TEXT input_data
        TEXT expected_output
        NUMERIC points
        BOOLEAN is_sample
    }
    evaluation_criteria {
        SERIAL id PK
        INT problem_id FK
        VARCHAR name
        TEXT description
        NUMERIC max_points
        SMALLINT order_index
    }
    programming_languages {
        SMALLSERIAL id PK
        VARCHAR name UK
        VARCHAR file_extension
        VARCHAR compile_command
        VARCHAR run_command
        BOOLEAN is_enabled
    }
    submissions {
        BIGSERIAL id PK
        INT problem_id FK
        INT team_id FK
        INT user_id FK
        SMALLINT language_id FK
        TEXT source_code
        VARCHAR repository_url
        CHAR commit_hash
        VARCHAR status
        NUMERIC score
        TEXT compile_output
        INT max_time_ms
        INT max_memory_kb
        TIMESTAMPTZ submitted_at
        TIMESTAMPTZ judged_at
    }
    submission_test_results {
        BIGINT submission_id PK
        INT test_case_id PK
        VARCHAR verdict
        INT time_ms
        INT memory_kb
        NUMERIC points_awarded
        VARCHAR checker_message
    }
    jury_scores {
        SERIAL id PK
        BIGINT submission_id FK
        INT criterion_id FK
        INT jury_member_id FK
        NUMERIC points
        TEXT comment
        TIMESTAMPTZ scored_at
    }
```
