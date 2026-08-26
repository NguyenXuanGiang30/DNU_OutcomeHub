using Microsoft.EntityFrameworkCore;
using Npgsql;
using OutcomeHub.Application.Common.Models;
using OutcomeHub.Application.Common.Security;
using OutcomeHub.Domain.Entities.Academic;
using OutcomeHub.Infrastructure.Persistence;
using OutcomeHub.Infrastructure.Persistence.Interceptors;
using OutcomeHub.Infrastructure.Persistence.Repositories.Academic;
using OutcomeHub.Infrastructure.Persistence.Rls;
using OutcomeHub.Migrations;
using Testcontainers.PostgreSql;
using Xunit;

namespace OutcomeHub.DatabaseTests;

public sealed class AcademicApiIntegrationTests
{
    [Fact(Timeout = 180_000)]
    public async Task AcademicRepositoriesExecuteSuccessfullyWithinRlsContext()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using PostgreSqlContainer postgreSql = new PostgreSqlBuilder("postgres:18")
            .WithDatabase("outcomehub_api_tests")
            .WithUsername("outcomehub_test_owner")
            .WithPassword("outcomehub_test_owner_password")
            .Build();

        await postgreSql.StartAsync(cancellationToken);
        string ownerConnectionString = postgreSql.GetConnectionString();

        string migrationConnectionString = await DatabaseBaselineTests.ProvisionDatabaseRolesAsync(
            ownerConnectionString,
            cancellationToken);

        string migrationRoot = Path.Combine(AppContext.BaseDirectory, "MigrationSql");
        var runner = new SqlMigrationRunner(migrationConnectionString, migrationRoot);
        var migrationResult = await runner.RunAsync(cancellationToken);
        Assert.Equal(19, migrationResult.AppliedCount);

        // Run development seed dataset
        await DatabaseBaselineTests.RunDatabaseScriptAsync(
            ownerConnectionString,
            "seed_development_dataset.sql",
            cancellationToken);

        // Configure DbContext connected with outcomehub_app role (subject to RLS)
        var appConnectionString = new NpgsqlConnectionStringBuilder(ownerConnectionString)
        {
            Username = "outcomehub_app",
            Password = "outcomehub_test_app_password",
            Pooling = false,
        }.ConnectionString;

        var optionsBuilder = new DbContextOptionsBuilder<OutcomeHubDbContext>()
            .UseNpgsql(appConnectionString)
            .AddInterceptors(new RowVersionSaveChangesInterceptor());

        await using var dbContext = new OutcomeHubDbContext(optionsBuilder.Options);
        var rlsExecutor = new RlsTransactionExecutor(dbContext);

        var orgUnitRepo = new OrgUnitRepository(dbContext);
        var programRepo = new ProgramRepository(dbContext);
        var outcomeRepo = new OutcomeRepository(dbContext);

        // 1. Admin Context: Read OrgUnit Tree
        var adminPrincipalId = Guid.Parse("10000000-0000-7000-8000-000000000001");
        var adminContext = new DatabaseRequestContext(adminPrincipalId, Guid.NewGuid(), "Admin reads OrgUnit tree");

        var orgTree = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => orgUnitRepo.GetTreeAsync(ct),
            cancellationToken);

        Assert.NotEmpty(orgTree);
        var university = orgTree.First(u => u.Code == "DNU");
        Assert.Equal("Trường Đại học Đại Nam", university.Name);
        Assert.NotEmpty(university.Children);
        Assert.Contains(university.Children, c => c.Code == "FIT");
        Assert.Contains(university.Children, c => c.Code == "FAA");

        // 2. Read Programs & Filter by OwnerOrgUnitId
        var fitOrgId = university.Children.First(c => c.Code == "FIT").Id;
        var programs = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => programRepo.GetPagedAsync(new PagedRequest { PageSize = 10 }, fitOrgId, ct),
            cancellationToken);

        Assert.True(programs.TotalCount >= 1);
        var itProgram = programs.Items.First(p => p.Code == "7480201");
        Assert.Equal("Công nghệ thông tin", itProgram.Name);

        // 3. Create a new Program in FIT
        var newProgram = Program.Create(
            id: Guid.NewGuid(),
            code: "7480202",
            name: "An toàn thông tin",
            degreeLevel: "BACHELOR",
            educationMode: "FULL_TIME",
            ownerOrgUnitId: fitOrgId,
            status: "DRAFT",
            createdBy: adminPrincipalId);

        var createdProgram = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => programRepo.CreateAsync(newProgram, ct),
            cancellationToken);

        Assert.Equal("7480202", createdProgram.Code);
        Assert.Equal("An toàn thông tin", createdProgram.Name);

        // 4. Read Versions of IT Program
        var versions = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => programRepo.GetVersionsByProgramIdAsync(itProgram.Id, ct),
            cancellationToken);

        Assert.NotEmpty(versions);
        var k17Version = versions[0];
        Assert.Equal("7480201_K17", k17Version.Code);

        // 5. Create a new Program Version K18
        var k18WorkflowId = Guid.NewGuid();
        var defaultDefId = Guid.Parse("00000000-0000-7000-8000-000000000401");
        await rlsExecutor.ExecuteAsync(
            adminContext,
            async ct =>
            {
                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $"INSERT INTO workflow.instance (id, definition_id, current_state, started_by, started_at, row_version) VALUES ({k18WorkflowId}, {defaultDefId}, 'DRAFT', {adminPrincipalId}, CURRENT_TIMESTAMP, 1)",
                    ct);
                return true;
            },
            cancellationToken);

        var k18Version = ProgramVersion.Create(
            id: Guid.NewGuid(),
            programId: itProgram.Id,
            institutionTemplateVersionId: k17Version.InstitutionTemplateVersionId,
            versionNo: 2,
            code: "7480201_K18",
            decisionId: k17Version.DecisionId,
            effectiveFrom: new DateOnly(2024, 9, 1),
            effectiveTo: null,
            status: "DRAFT",
            totalCredits: 135.0m,
            workflowInstanceId: k18WorkflowId,
            supersedesId: k17Version.Id,
            checksum: Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"));

        var createdK18 = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => programRepo.CreateVersionAsync(k18Version, ct),
            cancellationToken);

        Assert.Equal("7480201_K18", createdK18.Code);
        Assert.Equal(2, createdK18.VersionNo);

        // 6. Read Outcomes Tree
        var outcomeTree = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => outcomeRepo.GetOutcomeTreeAsync(k17Version.Id, ct),
            cancellationToken);

        Assert.NotNull(outcomeTree);
        Assert.NotEmpty(outcomeTree.Plos);
        Assert.Contains(outcomeTree.Plos, plo => plo.Code == "PLO5");

        // 7. Create custom PLO
        var newPlo = ProgramPlo.Create(
            id: Guid.NewGuid(),
            programVersionId: k17Version.Id,
            code: "PLO10",
            description: "Năng lực phát triển hệ thống trí tuệ nhân tạo và dữ liệu lớn",
            domain: "KNOWLEDGE",
            bloomLevel: "CREATE",
            sourceTemplatePloId: null,
            isLocked: false,
            sortOrder: 10);

        var createdPlo = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => outcomeRepo.CreatePloAsync(newPlo, ct),
            cancellationToken);

        Assert.Equal("PLO10", createdPlo.Code);

        // 8. Create custom PI
        var newPi = ProgramPi.Create(
            id: Guid.NewGuid(),
            programVersionId: k17Version.Id,
            programPloId: createdPlo.Id,
            code: "PI10.1",
            description: "Thiết kế và triển khai mô hình học máy theo quy trình MLOps chuẩn",
            sourceTemplatePiId: null,
            isLocked: false,
            isCore: true,
            weightRatio: 1.0m,
            sortOrder: 1);

        var createdPi = await rlsExecutor.ExecuteAsync(
            adminContext,
            ct => outcomeRepo.CreatePiAsync(newPi, ct),
            cancellationToken);

        Assert.Equal("PI10.1", createdPi.Code);
        Assert.True(createdPi.IsCore);
    }
}
