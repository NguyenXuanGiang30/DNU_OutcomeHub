# OutcomeHub

Backend ASP.NET Core của hệ thống đo lường chuẩn đầu ra OBE, tổ chức theo Clean Architecture. Bốn layer nghiệp vụ được giữ độc lập với công cụ triển khai database:

```text
src/
├── OutcomeHub.Domain/          # Entity, Value Object, Enum, hợp đồng nghiệp vụ
├── OutcomeHub.Application/     # Use Case, DTO, Validator, application contract
├── OutcomeHub.Infrastructure/  # EF Core, PostgreSQL, repository, tích hợp ngoài
├── OutcomeHub.Api/             # Controller, Middleware, DI, host API
└── OutcomeHub.Migrations/      # Công cụ triển khai canonical SQL, không phải runtime layer

tests/
└── OutcomeHub.DatabaseTests/   # Testcontainers PostgreSQL 18 và database smoke test
```

## Chiều phụ thuộc

```text
OutcomeHub.Api ───────────> OutcomeHub.Application ──> OutcomeHub.Domain
       │                              ▲
       └──> OutcomeHub.Infrastructure ┘
                         └──────────────> OutcomeHub.Domain

OutcomeHub.Migrations ───> PostgreSQL/Npgsql
```

`OutcomeHub.Migrations` là deployment tool độc lập. API không gọi migration khi khởi động và các layer nghiệp vụ không phụ thuộc project này.

## Trạng thái database

- Mô hình hiện có `250` Entity và `250` EF Core Configuration, tuân thủ nguyên tắc một Entity/một file và một Configuration/một file.
- Database mục tiêu là PostgreSQL `18`.
- Nguồn triển khai chuẩn nằm tại [`src/OutcomeHub.Migrations/Sql`](src/OutcomeHub.Migrations/Sql) và được điều phối bởi [`manifest.json`](src/OutcomeHub.Migrations/Sql/manifest.json).
- Runner kiểm SHA-256 của script/precondition/postcondition, khóa đồng thời bằng PostgreSQL advisory lock, chạy kiểm tra trước/sau và ghi ledger vào `ops.schema_migration`.
- Canonical database **không dùng** `public."__EFMigrationsHistory"`.

Catalog hiện có tám migration canonical:

1. `0001_baseline_20260825`
2. `0002_database_hardening`
3. `0003_critical_business_invariants`
4. `0004_org_owned_roots_rls`
5. `0005_snapshot_result_immutability`
6. `0006_score_record_read_rls`
7. `0007_syllabus_and_offering_rls`
8. `0008_result_and_student_self_rls`

Đây chưa phải tuyên bố toàn bộ database đã hoàn tất. Một số hạng mục như mở rộng RLS cho dữ liệu nhạy cảm/kết quả, classification, worker authorization, partitioning và vận hành audit/retention vẫn cần được hoàn thiện và kiểm chứng trước khi làm logic/API tương ứng.

## Build và kiểm thử

Yêu cầu .NET SDK theo [`global.json`](global.json), Docker đang chạy và PostgreSQL image có thể được tải về:

```bash
dotnet restore OutcomeHub.slnx
dotnet build OutcomeHub.slnx --no-restore
dotnet test tests/OutcomeHub.DatabaseTests/OutcomeHub.DatabaseTests.csproj \
  --no-build --no-restore
```

Database test tạo PostgreSQL 18 tạm thời bằng Testcontainers, chạy hai migration runner đồng thời, kiểm tra lần chạy lại không thay đổi dữ liệu, phát hiện checksum drift và thực thi các smoke test về RLS, invariant, hardening và tính bất biến của snapshot/kết quả.

## Chạy API

Thiết lập `ConnectionStrings__OutcomeHub` bằng ASP.NET User Secrets hoặc secret manager; không ghi connection string hay password vào source code. Sau đó chạy:

```bash
dotnet run --project src/OutcomeHub.Api/OutcomeHub.Api.csproj
```

Endpoint kiểm tra: `GET /api/v1/system/status`.

Quy trình tạo role, quản lý secret và chạy canonical migration được mô tả tại [`docs/DEV_PostgreSQL_Development.md`](docs/DEV_PostgreSQL_Development.md).
