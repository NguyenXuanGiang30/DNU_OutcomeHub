# PostgreSQL Development cho OutcomeHub

## Phạm vi

Tài liệu này mô tả cách khởi tạo PostgreSQL 18, provision ba role của ứng dụng và chạy bộ canonical SQL migration. API không tự chạy migration khi khởi động.

Repository đã có runner và sáu migration canonical cùng bộ kiểm thử cho database PostgreSQL 18 sạch. Trạng thái của named volume trên từng máy là độc lập; không được suy ra rằng volume Development đã được nâng cấp chỉ vì source code đã có migration mới.

## Thành phần hiện tại

- PostgreSQL `18` chạy bằng Docker Compose tại `localhost:5432` theo cấu hình mặc định.
- Database mặc định: `outcomehub`.
- Bootstrap admin mặc định: `outcomehub`; chỉ dùng để khởi tạo môi trường Development và provision role, không dùng làm tài khoản runtime.
- `outcomehub_authorizer`: `NOLOGIN`, `NOINHERIT`, `NOBYPASSRLS`; sở hữu các hàm kiểm quyền để application role không được đọc trực tiếp dữ liệu IAM bên dưới.
- `outcomehub_app`: tài khoản kết nối của API; `NOINHERIT`, `NOBYPASSRLS` và chỉ nhận quyền/policy tối thiểu đã cấp.
- `outcomehub_migrator`: tài khoản triển khai; không phải superuser, là thành viên của authorizer role và là owner database canonical.
- Password Development nằm trong `.secrets/`; thư mục này đã được loại khỏi source control.
- Connection string API được cung cấp qua ASP.NET User Secrets hoặc secret manager.

Không commit `.env`, password, connection string đầy đủ hoặc nội dung `.secrets/`. Không bật shell tracing (`set -x`) khi đang nạp secret.

## Chuẩn bị secret

Sao chép `.env.example` thành `.env` và tạo ba file chỉ chứa đúng một password:

```text
.secrets/postgres_password
.secrets/app_password
.secrets/migrator_password
```

Giới hạn quyền đọc cho tài khoản hiện tại:

```bash
chmod 700 .secrets
chmod 600 .secrets/postgres_password \
  .secrets/app_password \
  .secrets/migrator_password
```

Docker Compose mount các file trên bằng Docker secrets. Hai bootstrap script đọc password trực tiếp từ `/run/secrets/...` ở phía PostgreSQL; password không được truyền bằng đối số `psql` và không xuất hiện trong log lệnh.

## Khởi động PostgreSQL và provision role

```bash
docker compose up -d database
docker compose ps
docker compose exec -T database \
  pg_isready --username outcomehub --dbname outcomehub
```

Provision authorizer/application role trước, sau đó provision migrator:

```bash
docker compose exec -T database \
  psql -X -v ON_ERROR_STOP=1 -U outcomehub -d outcomehub -f - \
  < database/development/bootstrap_app_role.sql

docker compose exec -T database \
  psql -X -v ON_ERROR_STOP=1 -U outcomehub -d outcomehub -f - \
  < database/development/bootstrap_migrator_role.sql
```

Các script idempotent đối với role đã tồn tại: chúng khóa lại thuộc tính role và cập nhật password từ secret file. Script migrator chuyển owner của database hiện tại sang `outcomehub_migrator`.

Nếu `.env` thay đổi tên database hoặc bootstrap user, thay các giá trị `-d`/`-U` trong lệnh bằng cấu hình tương ứng.

## Canonical SQL migration

Nguồn migration chuẩn:

```text
src/OutcomeHub.Migrations/
├── Program.cs
├── SqlMigrationRunner.cs
└── Sql/
    ├── bootstrap.sql
    ├── manifest.json
    ├── manifest.schema.json
    ├── transactional/
    └── checks/
```

Runner thực hiện các kiểm soát sau:

- chỉ chấp nhận PostgreSQL major version khai báo trong manifest (`18`);
- xác thực thứ tự/id/tên migration và SHA-256 riêng cho script, precondition, postcondition;
- từ chối path thoát khỏi migration root, UTF-8 BOM, CRLF và `psql` meta-command trong artifact;
- dùng session advisory lock cố định để chỉ một deployment thay đổi schema tại một thời điểm;
- chạy precondition, script, postcondition và cập nhật ledger trong cùng transaction đối với migration `TRANSACTIONAL`;
- chạy lại postcondition cho migration đã áp dụng, phát hiện checksum/ledger drift và yêu cầu ledger là prefix liên tục của manifest.

Ledger canonical là `ops.schema_migration`. Bảng này được bootstrap và bảo vệ khỏi sửa/xóa/truncate tùy tiện. Canonical database không tạo hoặc sử dụng `public."__EFMigrationsHistory"`.

### Sáu migration hiện có

1. `0001_baseline_20260825`: baseline schema, constraint/index và nền tảng RLS Course.
2. `0002_database_hardening`: bổ sung bất biến dữ liệu, bảo vệ dữ liệu append-only/immutable, outbox và kiểm tra scope score.
3. `0003_critical_business_invariants`: khóa quan hệ chéo aggregate, typed binding và ngăn cycle ở các cấu trúc phân cấp/supersession.
4. `0004_org_owned_roots_rls`: mở rộng permission và FORCE RLS cho nhóm bảng gốc sở hữu theo đơn vị/chương trình.
5. `0005_snapshot_result_immutability`: khóa 12 bảng con snapshot sau khi seal và 11 bảng kết quả cuối sau khi batch được finalize, đồng thời chống race giữa writer và seal/finalize.
6. `0006_score_record_read_rls`: bật FORCE RLS cho `measurement.score_record` và chỉ cấp SELECT theo scope tổ chức/chương trình/học phần; không cấp quyền ghi trực tiếp cho API role.

### Build và chạy migration không lộ password

Build trước:

```bash
dotnet restore OutcomeHub.slnx
dotnet build src/OutcomeHub.Migrations/OutcomeHub.Migrations.csproj --no-restore
```

Trong Development mặc định, đọc password vào biến cục bộ trong subshell rồi truyền connection string qua biến môi trường mà runner yêu cầu:

```bash
(
  IFS= read -r outcomehub_migrator_password \
    < .secrets/migrator_password
  export OUTCOMEHUB_MIGRATIONS_CONNECTION_STRING="Host=localhost;Port=5432;Database=outcomehub;Username=outcomehub_migrator;Password=${outcomehub_migrator_password}"

  dotnet run --project src/OutcomeHub.Migrations/OutcomeHub.Migrations.csproj \
    --no-build
)
```

Password không nằm trong command line, source hoặc output. Subshell hủy biến sau khi runner kết thúc. Với CI/CD, inject `OUTCOMEHUB_MIGRATIONS_CONNECTION_STRING` từ secret manager dưới dạng masked secret; không ghép giá trị secret vào YAML hay log.

Runner cũng hỗ trợ các tùy chọn sau phần `--` khi thật sự cần:

```text
--connection-env <ENV_NAME>
--migrations-dir <PATH>
--lock-timeout-seconds <SECONDS>
```

Không sửa script/checksum của migration đã được áp dụng. Mọi thay đổi schema tiếp theo phải là migration mới, có precondition, postcondition và checksum mới trong manifest.

## Cảnh báo đối với database legacy

`0001_baseline_20260825` dành cho target sạch theo precondition của baseline. Nếu volume cũ còn ba EF migration (`InitialOutcomeHubSchema`, `AddPostgreSqlIntegrityFoundation`, `AddCourseRlsFoundation`) hoặc có `public."__EFMigrationsHistory"`, không chạy canonical baseline trực tiếp lên đó.

Trước khi chuyển đổi phải:

1. sao lưu database/volume hiện tại;
2. tạo database canonical song song hoặc khôi phục bản sao vào môi trường thử;
3. chạy runner và toàn bộ database test trên target thử;
4. lập kế hoạch chuyển dữ liệu/cutover riêng rồi mới thay Development target.

Kiểm tra nhanh loại database đang dùng:

```bash
docker compose exec -T database \
  psql -X -U outcomehub -d outcomehub \
  -c "SELECT to_regclass('ops.schema_migration') AS canonical_ledger, to_regclass('public.\"__EFMigrationsHistory\"') AS legacy_ef_history;"
```

Không xóa named volume để xử lý khác biệt migration nếu chưa có chủ ý và bản sao lưu.

## Kiểm thử database

Chạy bộ test tự động thay vì tạo fixture trên database Development:

```bash
dotnet build OutcomeHub.slnx --no-restore
dotnet test tests/OutcomeHub.DatabaseTests/OutcomeHub.DatabaseTests.csproj \
  --no-build --no-restore
```

Docker daemon phải hoạt động. Testcontainers tạo PostgreSQL 18 tạm thời, provision đủ ba role, chạy hai runner đồng thời và kiểm tra:

- sáu migration được áp dụng đúng một lần;
- lần chạy lại là no-op nhưng vẫn qua postcondition;
- checksum drift bị từ chối;
- `__EFMigrationsHistory` không tồn tại;
- smoke test Course RLS, database hardening, critical business invariants, org-owned roots RLS, score-record RLS và snapshot/result immutability đều đạt;
- fixture nằm trong transaction và được rollback.

Các file smoke test nằm tại `database/tests/`. Không dùng fixture của test làm seed Development.

## Kiểm tra API

Sau khi cấu hình `ConnectionStrings__OutcomeHub` bằng User Secrets/secret manager và chạy API:

```bash
curl http://localhost:5080/api/v1/system/database
```

Kết quả mong đợi khi kết nối thành công:

```json
{"database":"PostgreSQL","status":"Healthy"}
```

## Trạng thái chưa hoàn tất

Canonical baseline, hardening, các invariant trọng yếu, ba wave RLS đầu và immutability của snapshot/kết quả đã có test tự động. Database vẫn còn backlog trước khi có thể coi là hoàn chỉnh cho toàn hệ thống, gồm tối thiểu:

- RLS/permission cho các bảng nhạy cảm, kết quả đo lường, evidence/document, CQI, AI, audit và integration còn lại (wave hiện tại mới khóa đọc `measurement.score_record`);
- classification clearance, masking PII và worker/job authorization;
- partitioning cho score/result facts và chiến lược vận hành partition;
- audit append API, retention/purge có kiểm soát, backup/restore và diễn tập deployment/cutover.

Chỉ chuyển sang logic/API của một module khi schema, invariant, quyền truy cập và test database của module đó đã được khóa rõ ràng.

## Dừng database

```bash
docker compose stop database
```

Lệnh trên giữ nguyên named volume. Không dùng `docker compose down -v` nếu chưa chủ ý xóa toàn bộ dữ liệu Development và đã xác nhận khả năng khôi phục.
