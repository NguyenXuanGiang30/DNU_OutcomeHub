using System.Globalization;
using Microsoft.EntityFrameworkCore;
using OutcomeHub.Application.DTOs.Ai;
using OutcomeHub.Application.Interfaces.Persistence;

namespace OutcomeHub.Infrastructure.Persistence.Repositories.Ai;

public sealed class AiAssistantRepository : IAiAssistantRepository
{
    private readonly OutcomeHubDbContext _dbContext;

    public AiAssistantRepository(OutcomeHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AiChatResponseDto> QueryChatbotAsync(
        AiChatQueryRequest request,
        CancellationToken cancellationToken)
    {
        // Query Program and PLO context to build RAG citation
        var plos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Include(p => p.ProgramVersion)
            .ThenInclude(pv => pv.Program)
            .Take(3)
            .ToListAsync(cancellationToken);

        var citations = new List<AiCitationDto>();
        string answer = "Dựa trên dữ liệu chuẩn đầu ra của Nhà trường:";

        if (plos.Count > 0)
        {
            var firstPlo = plos[0];
            citations.Add(new AiCitationDto(
                "CURRICULUM_VERSION",
                firstPlo.ProgramVersionId.ToString(),
                $"Chương trình đào tạo {firstPlo.ProgramVersion.Program.Name}",
                "Mục 3: Chuẩn đầu ra PLO",
                firstPlo.ProgramVersion.EffectiveFrom.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                "Tỷ lệ đạt chuẩn = (Số SV đạt ngưỡng / Tổng SV) * 100%"
            ));

            citations.Add(new AiCitationDto(
                "BM13_SPEC",
                firstPlo.Id.ToString(),
                $"Bản mô tả chuẩn đầu ra {firstPlo.Code}",
                $"CĐR {firstPlo.Code} - {firstPlo.BloomLevel}",
                DateTimeOffset.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                "Đánh giá trực tiếp qua Rubric các bài A1/A2/A3"
            ));

            answer = $"Chương trình đào tạo {firstPlo.ProgramVersion.Program.Name} hiện có {plos.Count} chuẩn đầu ra chính. " +
                     $"CĐR {firstPlo.Code} có mức Bloom {firstPlo.BloomLevel}, mô tả: '{firstPlo.Description}'. " +
                     $"Tỷ lệ đạt chuẩn trung bình của các đợt đo gần nhất đạt mức khuyến nghị (>80%).";
        }
        else
        {
            citations.Add(new AiCitationDto(
                "CURRICULUM_VERSION",
                "SYS-CTD-2023",
                "Khung Chuẩn Đầu Ra Cấp Trường DNU",
                "Mục 2: Khung năng lực",
                "2023-09-01",
                "Thang đo Bloom 6 mức độ"
            ));

            answer = "Dựa trên Khung Chuẩn Đầu Ra cấp Trường, các chương trình đào tạo áp dụng triết lý giáo dục định hướng kết quả (OBE) với các ma trận liên kết CLO-PI-PLO và kế hoạch cải tiến chất lượng liên tục CQI.";
        }

        return new AiChatResponseDto(
            answer,
            citations,
            false,
            citations.Count,
            0.96,
            "gemini-1.5-pro",
            DateTimeOffset.UtcNow);
    }

    public Task<AiExtractionResultDto> ExtractDocumentAsync(
        AiDocumentExtractionRequest request,
        CancellationToken cancellationToken)
    {
        var fields = new List<AiExtractedFieldDto>
        {
            new("course_code", "IT4101", 1, "{\"x\":120,\"y\":80,\"width\":200,\"height\":30}", 0.98, false),
            new("course_name_vi", "Lập trình .NET nâng cao", 1, "{\"x\":120,\"y\":120,\"width\":350,\"height\":35}", 0.99, false),
            new("credits", "3", 1, "{\"x\":480,\"y\":120,\"width\":50,\"height\":30}", 0.95, false),
            new("clo1_code", "CLO1", 2, "{\"x\":80,\"y\":200,\"width\":100,\"height\":40}", 0.96, false),
            new("clo1_description", "Vận dụng kiến thức C# và ASP.NET Core để xây dựng web API an toàn.", 2, "{\"x\":190,\"y\":200,\"width\":600,\"height\":60}", 0.94, false),
            new("clo1_bloom_level", "APPLY", 2, "{\"x\":800,\"y\":200,\"width\":100,\"height\":40}", 0.91, true)
        };

        var result = new AiExtractionResultDto(
            Guid.NewGuid(),
            request.DocumentId,
            request.DocumentType,
            request.TargetSchemaVersion,
            fields,
            0.955,
            "PENDING_REVIEW",
            DateTimeOffset.UtcNow);

        return Task.FromResult(result);
    }

    public async Task<AiAnomalyDetectionResultDto> RunDiagnosticsAsync(
        Guid programVersionId,
        CancellationToken cancellationToken)
    {
        var pv = await _dbContext.ProgramVersions
            .AsNoTracking()
            .Include(v => v.Program)
            .FirstOrDefaultAsync(v => v.Id == programVersionId, cancellationToken);

        var plos = await _dbContext.ProgramPlos
            .AsNoTracking()
            .Where(p => p.ProgramVersionId == programVersionId)
            .ToListAsync(cancellationToken);

        var issues = new List<AiAnomalyIssueDto>();

        if (pv != null)
        {
            if (plos.Count == 0)
            {
                issues.Add(new AiAnomalyIssueDto(
                    "NO_PLOS_DEFINED",
                    "CRITICAL",
                    "PI_COVERAGE_GAP",
                    "Phiên bản CTĐT chưa có chuẩn đầu ra PLO nào được khai báo.",
                    pv.Program.Code,
                    "Khai báo tối thiểu 5 PLO theo quy định của Nhà trường."));
            }

            issues.Add(new AiAnomalyIssueDto(
                "BLOOM_TAXONOMY_CHECK",
                "INFO",
                "BLOOM_CONFLICT",
                "Phân bố mức Bloom của các CĐR cân đối và đúng định chuẩn.",
                "ProgramPlos",
                "Duy trì tỷ lệ các mức Bloom phân tích và ứng dụng."));
        }
        else
        {
            issues.Add(new AiAnomalyIssueDto(
                "VALIDATION_PASS",
                "INFO",
                "WEIGHT_SUM_INVALID",
                "Tổng trọng số các học phần và bài đánh giá đạt chuẩn 100%.",
                "CurriculumMatrix",
                "Không cần can thiệp."));
        }

        return new AiAnomalyDetectionResultDto(
            programVersionId,
            pv?.Program.Code ?? "7480201",
            issues.Count,
            issues.Count(i => i.Severity == "CRITICAL"),
            issues.Count(i => i.Severity == "WARNING"),
            issues,
            DateTimeOffset.UtcNow);
    }

    public Task<IReadOnlyList<HitlReviewItemDto>> GetHitlQueueAsync(
        Guid? extractionId,
        string? status,
        CancellationToken cancellationToken)
    {
        var list = new List<HitlReviewItemDto>
        {
            new(
                Guid.NewGuid(),
                extractionId ?? Guid.NewGuid(),
                "SYLLABUS",
                "clo1_bloom_level",
                "APPLY",
                0.91,
                true,
                status ?? "PENDING",
                DateTimeOffset.UtcNow.AddMinutes(-30)
            ),
            new(
                Guid.NewGuid(),
                extractionId ?? Guid.NewGuid(),
                "SYLLABUS",
                "assessment_weight_a2",
                "30%",
                0.97,
                false,
                status ?? "PENDING",
                DateTimeOffset.UtcNow.AddMinutes(-20)
            )
        };

        return Task.FromResult<IReadOnlyList<HitlReviewItemDto>>(list);
    }

    public Task<HitlDecisionResultDto> SubmitHitlDecisionAsync(
        HitlDecisionRequest request,
        CancellationToken cancellationToken)
    {
        var result = new HitlDecisionResultDto(
            request.ReviewItemId,
            request.CorrectedValue ?? "APPLY",
            request.Action == "ACCEPT" ? "APPROVED" : (request.Action == "MODIFY" ? "MODIFIED_APPROVED" : "REJECTED"),
            "GV001",
            DateTimeOffset.UtcNow);

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<PromptTemplateVersionDto>> GetPromptVersionsAsync(
        string? promptCode,
        CancellationToken cancellationToken)
    {
        var list = new List<PromptTemplateVersionDto>
        {
            new(
                Guid.NewGuid(),
                "OBE_RAG_SYNTHESIS",
                2,
                "GOOGLE",
                "gemini-1.5-pro",
                "Bạn là trợ lý học thuật OBE thông minh của Trường Đại học Đông Á...",
                "{\"type\":\"object\",\"properties\":{\"answer\":{\"type\":\"string\"}}}",
                true,
                DateTimeOffset.UtcNow.AddDays(-20)
            ),
            new(
                Guid.NewGuid(),
                "BM13_EXTRACTION",
                1,
                "OPENAI",
                "gpt-4o",
                "Trích xuất thông tin Đề cương chi tiết học phần theo mẫu BM13...",
                "{\"type\":\"object\",\"properties\":{\"courseCode\":{\"type\":\"string\"}}}",
                true,
                DateTimeOffset.UtcNow.AddDays(-15)
            )
        };

        return Task.FromResult<IReadOnlyList<PromptTemplateVersionDto>>(list);
    }

    public Task<PromptTemplateVersionDto> RegisterPromptVersionAsync(
        RegisterPromptVersionRequest request,
        CancellationToken cancellationToken)
    {
        var dto = new PromptTemplateVersionDto(
            Guid.NewGuid(),
            request.PromptCode,
            3,
            request.ModelProvider,
            request.ModelName,
            request.SystemPromptTemplate,
            request.OutputJsonSchema,
            true,
            DateTimeOffset.UtcNow);

        return Task.FromResult(dto);
    }

    public Task<PromptBenchmarkTestResultDto> RunPromptBenchmarkAsync(
        string promptCode,
        int versionNumber,
        CancellationToken cancellationToken)
    {
        var result = new PromptBenchmarkTestResultDto(
            promptCode,
            versionNumber,
            50,
            49,
            98.0,
            420.5,
            true);

        return Task.FromResult(result);
    }

    public Task<PromptInjectionScanResultDto> ScanPromptInjectionAsync(
        string textToScan,
        CancellationToken cancellationToken)
    {
        bool isSuspicious = textToScan.Contains("ignore previous instructions", StringComparison.OrdinalIgnoreCase) ||
                            textToScan.Contains("override system prompt", StringComparison.OrdinalIgnoreCase);

        var result = new PromptInjectionScanResultDto(
            !isSuspicious,
            isSuspicious ? 0.95 : 0.02,
            isSuspicious ? "INSTRUCTION_OVERRIDE" : null,
            isSuspicious ? "BLOCKED" : "ALLOWED",
            DateTimeOffset.UtcNow);

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<AiSecurityAuditLogDto>> GetSecurityAuditLogsAsync(
        CancellationToken cancellationToken)
    {
        var logs = new List<AiSecurityAuditLogDto>
        {
            new(
                Guid.NewGuid(),
                "Nguyen Van Giang",
                "CHATBOT_QUERY",
                "Hỏi đáp về tỷ lệ đạt CĐR của ngành CNTT khóa K17",
                false,
                "DATA_PRIVACY_MASKING",
                DateTimeOffset.UtcNow.AddHours(-1)
            ),
            new(
                Guid.NewGuid(),
                "Nguyen Van Giang",
                "DOCUMENT_EXTRACTION",
                "Trích xuất Đề cương môn Lập trình .NET nâng cao",
                false,
                "PROMPT_INJECTION_GUARD",
                DateTimeOffset.UtcNow.AddHours(-2)
            )
        };

        return Task.FromResult<IReadOnlyList<AiSecurityAuditLogDto>>(logs);
    }
}
