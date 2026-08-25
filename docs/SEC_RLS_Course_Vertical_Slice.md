# RLS vertical slice cho `academic.course`

## Mục tiêu

Wave đầu tiên chứng minh đầy đủ đường phân quyền:

```text
Request context trong transaction
→ Principal đang ACTIVE
→ RoleAssignment còn hiệu lực
→ RoleVersion đang ACTIVE
→ Permission khớp chính xác
→ AccessScope bao phủ row
→ PostgreSQL RLS cho phép hoặc từ chối
```

Không dùng wave này để suy ra rằng toàn bộ 250 bảng đã được cấp quyền. Các bảng chưa có policy vẫn không được cấp quyền cho `outcomehub_app`.

## Permission tuple đã khóa cho Course

| ResourceType | Action | FieldScope | SQL policy |
|---|---|---|---|
| `academic.course` | `READ` | `*` | `SELECT ... USING` |
| `academic.course` | `CREATE` | `*` | `INSERT ... WITH CHECK` |
| `academic.course` | `UPDATE` | `*` | `UPDATE ... USING/WITH CHECK` |
| `academic.course` | `DELETE` | `*` | `DELETE ... USING` |

So sánh permission phân biệt hoa/thường. ID seed cố định nằm trong migration `AddCourseRlsFoundation`.

## Scope được hỗ trợ trong wave 1

- `SYSTEM`: bao phủ mọi Course.
- `ORG_UNIT`: bao phủ Course của chính đơn vị; nếu `include_descendants = true` thì bao phủ cả đơn vị con. Recursive CTE có cycle guard.
- `COURSE`: chỉ bao phủ Course có ID đúng bằng anchor.

`PROGRAM`, `PROGRAM_VERSION`, `COHORT`, `CURRICULUM_PATH`, `OFFERING`, `MEASUREMENT_PERIOD`, `SELF` và `classification` chưa được hàm wave 1 cho phép. Các dimension này mặc định trả `false` cho tới khi ma trận bao phủ được chốt.

RoleVersion chỉ cấp quyền khi ở trạng thái `ACTIVE`, không dùng `APPROVED` như một trạng thái runtime.

## Request context

Mọi truy vấn Course bằng tài khoản ứng dụng phải chạy qua `IRlsTransactionExecutor`. Executor mở transaction trước, sau đó đặt bốn GUC bằng `set_config(..., true)` trên đúng connection:

```csharp
var context = new DatabaseRequestContext(
    principalId,
    requestId,
    "COURSE_LIST");

var courses = await rlsTransactionExecutor.ExecuteAsync(
    context,
    cancellationToken => courseRepository.ListAsync(cancellationToken),
    cancellationToken);
```

Không dùng session-level `SET`. Không gọi repository nhạy cảm ngoài executor. Authentication/API middleware phải tạo `principalId` từ identity đã xác thực; custom GUC không phải credential.

## Kiểm thử

Smoke test chạy trên PostgreSQL thật, tạo fixture trong transaction và rollback toàn bộ:

```bash
docker compose exec -T database \
  psql -U outcomehub -d outcomehub -f - \
  < database/tests/course_rls_smoke.sql
```

Test bao phủ:

- context thiếu, rỗng, UUID sai và purpose thiếu;
- principal không có assignment;
- scope đơn vị chính xác và đơn vị con;
- cách ly giữa Khoa A và Khoa B;
- `INSERT`/`UPDATE` chéo scope bị `WITH CHECK` từ chối;
- reader không thể ghi;
- application role không đọc trực tiếp bảng IAM hoặc kế thừa authorizer;
- `row_security=off` không bypass được `FORCE RLS`;
- GUC transaction-local không rò sang transaction kế tiếp.

## Giới hạn tiếp theo cần xử lý

- Chưa có authentication và ánh xạ claim IdP sang `iam.principal`.
- Chưa có role/assignment nghiệp vụ mặc định; smoke fixture không phải seed Development.
- Chưa triển khai worker context, BI binding, `SELF`, classification clearance hoặc masking PII.
- Chưa nhân policy sang score, snapshot, result, document/evidence, audit, CQI và AI.
- Chưa có Testcontainers/xUnit để kiểm connection-pool reuse tự động trong CI.
