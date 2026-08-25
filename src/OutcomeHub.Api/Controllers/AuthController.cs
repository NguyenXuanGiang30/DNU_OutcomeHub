using Microsoft.AspNetCore.Mvc;
using OutcomeHub.Application.Common.Models;

namespace OutcomeHub.Api.Controllers;

public sealed record TestActorDto(
    Guid PrincipalId,
    string Code,
    string Name,
    string Role,
    Guid? OrgUnitId,
    string Description);

public sealed record UserProfileDto(
    Guid PrincipalId,
    string? UserCode,
    string? FullName,
    string? RoleName,
    Guid? OrgUnitId,
    bool IsAuthenticated);

public sealed class AuthController : ApiControllerBase
{
    private static readonly IReadOnlyList<TestActorDto> TestActors =
    [
        new(
            Guid.Parse("10000000-0000-7000-8000-000000000001"),
            "ADMIN",
            "Quản trị viên Hệ thống (System Admin)",
            "SYSTEM_ADMIN",
            Guid.Parse("00000000-0000-7000-8000-000000000001"),
            "Toàn quyền quản trị cấp Trường"),
        new(
            Guid.Parse("10000000-0000-7000-8000-000000000010"),
            "DEAN_IT",
            "TS. Nguyễn Văn A (Trưởng Khoa CNTT)",
            "DEAN",
            Guid.Parse("00000000-0000-7000-8000-000000000002"),
            "Quản lý CTĐT CNTT & Học phần IT4101"),
        new(
            Guid.Parse("10000000-0000-7000-8000-000000000020"),
            "DEAN_ACC",
            "PGS.TS. Trần Thị B (Trưởng Khoa Kế toán)",
            "DEAN",
            Guid.Parse("00000000-0000-7000-8000-000000000003"),
            "Quản lý CTĐT Kế toán & Học phần ACC4104"),
        new(
            Guid.Parse("10000000-0000-7000-8000-000000000011"),
            "GV_IT",
            "ThS. Lê Văn C (Giảng viên CNTT)",
            "LECTURER",
            Guid.Parse("00000000-0000-7000-8000-000000000002"),
            "Giảng dạy & chấm điểm Lớp học phần IT4101"),
        new(
            Guid.Parse("10000000-0000-7000-8000-000000000021"),
            "GV_ACC",
            "ThS. Phạm Thị D (Giảng viên Kế toán)",
            "LECTURER",
            Guid.Parse("00000000-0000-7000-8000-000000000003"),
            "Giảng dạy & chấm điểm Lớp học phần ACC4104"),
        new(
            Guid.Parse("10000000-0000-7000-8000-000000000031"),
            "SV001",
            "Nguyễn Văn An (Sinh viên K17 CNTT)",
            "STUDENT",
            null,
            "Tra cứu điểm và chuẩn đầu ra cá nhân (Scope SELF)"),
        new(
            Guid.Parse("10000000-0000-7000-8000-000000000032"),
            "SV002",
            "Trần Văn Bình (Sinh viên K17 Kế toán)",
            "STUDENT",
            null,
            "Tra cứu điểm và chuẩn đầu ra cá nhân (Scope SELF)"),
    ];

    [HttpGet("actors")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TestActorDto>>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<IReadOnlyList<TestActorDto>>> GetTestActors()
    {
        return OkResponse(TestActors, "Danh sách các tài khoản kiểm thử mẫu cho môi trường phát triển.");
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<UserProfileDto>> GetCurrentUser()
    {
        var profile = new UserProfileDto(
            PrincipalId: CurrentUser.PrincipalId,
            UserCode: CurrentUser.UserCode,
            FullName: CurrentUser.FullName,
            RoleName: CurrentUser.RoleName,
            OrgUnitId: CurrentUser.OrgUnitId,
            IsAuthenticated: CurrentUser.IsAuthenticated);

        return OkResponse(profile, "Thông tin người dùng hiện tại.");
    }
}
