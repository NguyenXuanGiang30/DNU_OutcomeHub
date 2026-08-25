-- ==============================================================================
-- OutcomeHub: Bộ dữ liệu mẫu Development / Demo (DNU OBE Sample Dataset)
-- Mô hình chuẩn: ĐH Đại Nam - Khoa CNTT & Khoa Kế toán (Học phần mẫu: ACC4104, IT4101)
-- ==============================================================================

\set ON_ERROR_STOP on

BEGIN;

SET LOCAL session_replication_role = replica;

-- 1. Quản trị viên hệ thống & Người dùng (IAM Principals)
INSERT INTO iam.principal (
    id,
    principal_type,
    status,
    display_name,
    created_at)
VALUES
    ('10000000-0000-7000-8000-000000000001', 'USER', 'ACTIVE', 'Quản trị viên Hệ thống (System Admin)', CURRENT_TIMESTAMP),
    ('10000000-0000-7000-8000-000000000010', 'USER', 'ACTIVE', 'TS. Nguyễn Văn A (Trưởng Khoa CNTT)', CURRENT_TIMESTAMP),
    ('10000000-0000-7000-8000-000000000020', 'USER', 'ACTIVE', 'PGS.TS. Trần Thị B (Trưởng Khoa Kế toán)', CURRENT_TIMESTAMP),
    ('10000000-0000-7000-8000-000000000011', 'USER', 'ACTIVE', 'ThS. Lê Văn C (Giảng viên CNTT)', CURRENT_TIMESTAMP),
    ('10000000-0000-7000-8000-000000000021', 'USER', 'ACTIVE', 'ThS. Phạm Thị D (Giảng viên Kế toán)', CURRENT_TIMESTAMP),
    ('10000000-0000-7000-8000-000000000031', 'USER', 'ACTIVE', 'Nguyễn Văn An (Sinh viên K17 CNTT)', CURRENT_TIMESTAMP),
    ('10000000-0000-7000-8000-000000000032', 'USER', 'ACTIVE', 'Trần Văn Bình (Sinh viên K17 Kế toán)', CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;

-- 2. Cây đơn vị tổ chức (Org Units)
INSERT INTO academic.org_unit (
    id,
    parent_id,
    code,
    name,
    unit_type,
    effective_from,
    effective_to,
    status,
    created_at,
    created_by,
    updated_at,
    updated_by,
    row_version)
VALUES
    (
        '00000000-0000-7000-8000-000000000001',
        NULL,
        'DNU',
        'Trường Đại học Đại Nam',
        'UNIVERSITY',
        DATE '2020-01-01',
        NULL,
        'ACTIVE',
        CURRENT_TIMESTAMP,
        '10000000-0000-7000-8000-000000000001',
        CURRENT_TIMESTAMP,
        '10000000-0000-7000-8000-000000000001',
        1
    ),
    (
        '00000000-0000-7000-8000-000000000002',
        '00000000-0000-7000-8000-000000000001',
        'FIT',
        'Khoa Công nghệ Thông tin',
        'FACULTY',
        DATE '2020-01-01',
        NULL,
        'ACTIVE',
        CURRENT_TIMESTAMP,
        '10000000-0000-7000-8000-000000000001',
        CURRENT_TIMESTAMP,
        '10000000-0000-7000-8000-000000000001',
        1
    ),
    (
        '00000000-0000-7000-8000-000000000003',
        '00000000-0000-7000-8000-000000000001',
        'FAA',
        'Khoa Kế toán - Kiểm toán',
        'FACULTY',
        DATE '2020-01-01',
        NULL,
        'ACTIVE',
        CURRENT_TIMESTAMP,
        '10000000-0000-7000-8000-000000000001',
        CURRENT_TIMESTAMP,
        '10000000-0000-7000-8000-000000000001',
        1
    )
ON CONFLICT (id) DO NOTHING;

-- 3. Phân quyền truy cập Toàn quyền cho Admin (Access Scope & Roles)
INSERT INTO iam.access_scope (
    id,
    scope_type,
    org_unit_id,
    program_id,
    program_version_id,
    cohort_id,
    curriculum_path_id,
    course_id,
    course_offering_id,
    measurement_period_id,
    subject_principal_id,
    include_descendants,
    checksum,
    created_at)
VALUES (
    '00000000-0000-7000-8000-000000000101',
    'SYSTEM',
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    NULL,
    false,
    repeat('1', 64),
    CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;

INSERT INTO iam.role (id, code, name, is_system, status, created_at)
VALUES (
    '00000000-0000-7000-8000-000000000201',
    'SYSTEM_ADMIN_ROLE',
    'Quản trị viên toàn hệ thống',
    true,
    'ACTIVE',
    CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;

INSERT INTO workflow.definition (
    id,
    code,
    version_no,
    subject_type,
    configuration,
    effective_from,
    status,
    checksum)
VALUES (
    '00000000-0000-7000-8000-000000000401',
    'DEFAULT_APPROVAL_FLOW',
    1,
    'SYSTEM',
    '{"states": ["DRAFT", "APPROVED"]}'::jsonb,
    DATE '2020-01-01',
    'ACTIVE',
    repeat('a', 64))
ON CONFLICT (id) DO NOTHING;

INSERT INTO workflow.instance (
    id,
    definition_id,
    current_state,
    started_by,
    started_at,
    row_version)
VALUES (
    '00000000-0000-7000-8000-000000000402',
    '00000000-0000-7000-8000-000000000401',
    'APPROVED',
    '10000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP,
    1)
ON CONFLICT (id) DO NOTHING;

INSERT INTO iam.sod_policy_version (
    id,
    version_no,
    status,
    effective_from,
    effective_to,
    workflow_instance_id,
    checksum)
VALUES (
    '00000000-0000-7000-8000-000000000601',
    1,
    'ACTIVE',
    DATE '2020-01-01',
    NULL,
    '00000000-0000-7000-8000-000000000402',
    repeat('e', 64))
ON CONFLICT (id) DO NOTHING;

INSERT INTO iam.role_version (
    id,
    role_id,
    version_no,
    status,
    effective_from,
    effective_to,
    workflow_instance_id,
    decision_id,
    permission_set_checksum,
    checksum,
    created_by,
    created_at)
VALUES (
    '00000000-0000-7000-8000-000000000301',
    '00000000-0000-7000-8000-000000000201',
    1,
    'ACTIVE',
    DATE '2020-01-01',
    NULL,
    '00000000-0000-7000-8000-000000000402',
    NULL,
    repeat('2', 64),
    repeat('3', 64),
    '10000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;

INSERT INTO iam.role_version_permission (
    role_version_id,
    permission_id,
    granted_at,
    granted_by)
SELECT
    '00000000-0000-7000-8000-000000000301'::uuid,
    p.id,
    CURRENT_TIMESTAMP,
    '10000000-0000-7000-8000-000000000001'::uuid
FROM iam.permission AS p
ON CONFLICT DO NOTHING;

INSERT INTO iam.role_assignment (
    id,
    principal_id,
    role_id,
    role_version_id,
    access_scope_id,
    effective_from,
    effective_to,
    status,
    source,
    source_reference,
    granted_by,
    approved_by,
    workflow_instance_id,
    sod_policy_version_id,
    authorization_snapshot_checksum,
    requested_by,
    requested_at,
    approved_at,
    revoked_at,
    reason,
    revoke_reason)
VALUES (
    '00000000-0000-7000-8000-000000000501',
    '10000000-0000-7000-8000-000000000001',
    '00000000-0000-7000-8000-000000000201',
    '00000000-0000-7000-8000-000000000301',
    '00000000-0000-7000-8000-000000000101',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    CURRENT_TIMESTAMP + INTERVAL '365 days',
    'ACTIVE',
    'MANUAL',
    NULL,
    '10000000-0000-7000-8000-000000000001',
    '10000000-0000-7000-8000-000000000001',
    '00000000-0000-7000-8000-000000000402',
    '00000000-0000-7000-8000-000000000601',
    repeat('4', 64),
    '10000000-0000-7000-8000-000000000001',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    CURRENT_TIMESTAMP - INTERVAL '1 day',
    NULL,
    'Gán quyền System Admin',
    NULL)
ON CONFLICT (id) DO NOTHING;

-- 4. Chương trình đào tạo (Program)
INSERT INTO academic.program (
    id,
    code,
    name,
    degree_level,
    education_mode,
    owner_org_unit_id,
    status,
    created_at,
    created_by,
    updated_at,
    updated_by,
    row_version)
VALUES
    (
        '30000000-0000-7000-8000-000000000001',
        '7480201',
        'Công nghệ thông tin',
        'BACHELOR',
        'FULL_TIME',
        '00000000-0000-7000-8000-000000000002',
        'ACTIVE',
        CURRENT_TIMESTAMP,
        '10000000-0000-7000-8000-000000000001',
        CURRENT_TIMESTAMP,
        '10000000-0000-7000-8000-000000000001',
        1
    ),
    (
        '30000000-0000-7000-8000-000000000002',
        '7340301',
        'Kế toán',
        'BACHELOR',
        'FULL_TIME',
        '00000000-0000-7000-8000-000000000003',
        'ACTIVE',
        CURRENT_TIMESTAMP,
        '10000000-0000-7000-8000-000000000001',
        CURRENT_TIMESTAMP,
        '10000000-0000-7000-8000-000000000001',
        1
    )
ON CONFLICT (id) DO NOTHING;

-- 5. Quyết định ban hành & Khung chuẩn trường (Decision & Institution Template)
INSERT INTO academic.decision_record (
    id,
    decision_number,
    issued_on,
    issuer_org_unit_id,
    title,
    document_version_id,
    status,
    created_at)
VALUES (
    '50000000-0000-7000-8000-000000000001',
    'QD-123/QD-DNU',
    DATE '2023-08-15',
    '00000000-0000-7000-8000-000000000001',
    'Quyết định ban hành CTĐT K17 ngành CNTT và Kế toán',
    NULL,
    'ACTIVE',
    CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;

INSERT INTO academic.institution_template (
    id,
    code,
    name,
    owner_org_unit_id,
    description,
    created_at)
VALUES (
    '51000000-0000-7000-8000-000000000001',
    'DNU_CORE_TEMPLATE',
    'Khung Chuẩn đầu ra chung ĐH Đại Nam',
    '00000000-0000-7000-8000-000000000001',
    'Khung PLO1-PLO4 bắt buộc toàn trường',
    CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;

INSERT INTO academic.institution_template_version (
    id,
    institution_template_id,
    version_no,
    decision_id,
    effective_from,
    effective_to,
    status,
    layout_configuration,
    policy_configuration,
    workflow_instance_id,
    checksum,
    supersedes_id)
VALUES (
    '52000000-0000-7000-8000-000000000001',
    '51000000-0000-7000-8000-000000000001',
    1,
    '50000000-0000-7000-8000-000000000001',
    DATE '2023-09-01',
    NULL,
    'ACTIVE',
    '{"sections": ["GENERAL", "OUTCOMES"]}'::jsonb,
    '{"min_credits": 120}'::jsonb,
    '00000000-0000-7000-8000-000000000402',
    repeat('5', 64),
    NULL)
ON CONFLICT (id) DO NOTHING;

-- 6. Phiên bản CTĐT K17 (Program Version)
INSERT INTO academic.program_version (
    id,
    program_id,
    institution_template_version_id,
    version_no,
    code,
    decision_id,
    effective_from,
    effective_to,
    status,
    total_credits,
    workflow_instance_id,
    supersedes_id,
    checksum,
    row_version)
VALUES (
    '53000000-0000-7000-8000-000000000001',
    '30000000-0000-7000-8000-000000000001',
    '52000000-0000-7000-8000-000000000001',
    1,
    '7480201_K17',
    '50000000-0000-7000-8000-000000000001',
    DATE '2023-09-01',
    NULL,
    'ACTIVE',
    132.0,
    '00000000-0000-7000-8000-000000000402',
    NULL,
    repeat('6', 64),
    1)
ON CONFLICT (id) DO NOTHING;

-- 7. Chuẩn đầu ra (Program PLO & PI)
INSERT INTO academic.program_plo (
    id,
    program_version_id,
    code,
    description,
    domain,
    bloom_level,
    source_template_plo_id,
    is_locked,
    sort_order)
VALUES
    ('54000000-0000-7000-8000-000000000001', '53000000-0000-7000-8000-000000000001', 'PLO1', 'Nắm vững kiến thức khoa học cơ bản và thế giới quan khoa học', 'KNOWLEDGE', 'APPLY', NULL, false, 1),
    ('54000000-0000-7000-8000-000000000002', '53000000-0000-7000-8000-000000000001', 'PLO2', 'Kỹ năng ngoại ngữ đạt chuẩn B1 quốc tế', 'SKILL', 'APPLY', NULL, false, 2),
    ('54000000-0000-7000-8000-000000000003', '53000000-0000-7000-8000-000000000001', 'PLO3', 'Kỹ năng làm việc nhóm, giao tiếp và thuyết trình', 'SKILL', 'APPLY', NULL, false, 3),
    ('54000000-0000-7000-8000-000000000004', '53000000-0000-7000-8000-000000000001', 'PLO4', 'Đạo đức nghề nghiệp và trách nhiệm phục vụ cộng đồng', 'ATTITUDE', 'EVALUATE', NULL, false, 4),
    ('54000000-0000-7000-8000-000000000005', '53000000-0000-7000-8000-000000000001', 'PLO5', 'Phân tích, thiết kế và phát triển các giải pháp phần mềm chuyên sâu', 'KNOWLEDGE', 'CREATE', NULL, false, 5),
    ('54000000-0000-7000-8000-000000000006', '53000000-0000-7000-8000-000000000001', 'PLO6', 'Vận hành, bảo mật và tối ưu hạ tầng mạng và điện toán đám mây', 'SKILL', 'EVALUATE', NULL, false, 6)
ON CONFLICT (id) DO NOTHING;

INSERT INTO academic.program_pi (
    id,
    program_version_id,
    program_plo_id,
    code,
    description,
    source_template_pi_id,
    is_locked,
    is_core,
    weight_ratio,
    sort_order)
VALUES
    ('55000000-0000-7000-8000-000000000001', '53000000-0000-7000-8000-000000000001', '54000000-0000-7000-8000-000000000005', 'PI5.1', 'Thiết kế kiến trúc phần mềm hướng dịch vụ và mô hình cơ sở dữ liệu quan hệ', NULL, false, true, 0.5, 1),
    ('55000000-0000-7000-8000-000000000002', '53000000-0000-7000-8000-000000000001', '54000000-0000-7000-8000-000000000005', 'PI5.2', 'Lập trình backend RESTful API và frontend web responsive', NULL, false, true, 0.5, 2)
ON CONFLICT (id) DO NOTHING;

-- 8. Học phần (Course)
INSERT INTO academic.course (
    id,
    code,
    name,
    owner_org_unit_id,
    status)
VALUES
    (
        '40000000-0000-7000-8000-000000000001',
        'ACC4104',
        'Kế toán Máy',
        '00000000-0000-7000-8000-000000000003',
        'ACTIVE'
    ),
    (
        '40000000-0000-7000-8000-000000000002',
        'IT4101',
        'Lập trình .NET nâng cao',
        '00000000-0000-7000-8000-000000000002',
        'ACTIVE'
    )
ON CONFLICT (id) DO NOTHING;

SET LOCAL session_replication_role = origin;

COMMIT;

SELECT json_build_object(
    'status', 'seeded',
    'org_units_count', (SELECT count(*) FROM academic.org_unit),
    'programs_count', (SELECT count(*) FROM academic.program),
    'plos_count', (SELECT count(*) FROM academic.program_plo),
    'pis_count', (SELECT count(*) FROM academic.program_pi),
    'courses_count', (SELECT count(*) FROM academic.course));
