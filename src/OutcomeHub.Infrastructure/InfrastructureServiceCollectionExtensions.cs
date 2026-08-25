using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutcomeHub.Application.Interfaces.Persistence;
using OutcomeHub.Infrastructure.Persistence;
using OutcomeHub.Infrastructure.Persistence.Interceptors;
using OutcomeHub.Infrastructure.Persistence.Rls;

namespace OutcomeHub.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddSingleton<RowVersionSaveChangesInterceptor>();
        services.AddScoped<IRlsTransactionExecutor, RlsTransactionExecutor>();
        services.AddScoped<IOrgUnitRepository, Persistence.Repositories.Academic.OrgUnitRepository>();
        services.AddScoped<IProgramRepository, Persistence.Repositories.Academic.ProgramRepository>();
        services.AddScoped<IOutcomeRepository, Persistence.Repositories.Academic.OutcomeRepository>();
        services.AddScoped<ICourseRepository, Persistence.Repositories.Academic.CourseRepository>();
        services.AddScoped<ISyllabusRepository, Persistence.Repositories.Portfolio.SyllabusRepository>();
        services.AddScoped<ICloRepository, Persistence.Repositories.Portfolio.CloRepository>();
        services.AddScoped<IRubricRepository, Persistence.Repositories.Portfolio.RubricRepository>();
        services.AddScoped<IStudentRepository, Persistence.Repositories.Academic.StudentRepository>();
        services.AddScoped<IStaffRepository, Persistence.Repositories.Academic.StaffRepository>();
        services.AddScoped<ICourseOfferingRepository, Persistence.Repositories.Academic.CourseOfferingRepository>();
        services.AddScoped<ICohortRepository, Persistence.Repositories.Academic.CohortRepository>();
        services.AddScoped<IMeasurementPeriodRepository, Persistence.Repositories.Measurement.MeasurementPeriodRepository>();
        services.AddScoped<IEnrollmentRepository, Persistence.Repositories.Measurement.EnrollmentRepository>();
        services.AddScoped<IScoreRepository, Persistence.Repositories.Measurement.ScoreRepository>();

        services.AddDbContext<OutcomeHubDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("OutcomeHub");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Missing connection string 'ConnectionStrings:OutcomeHub'.");
            }

            options.UseNpgsql(connectionString);
            options.AddInterceptors(
                serviceProvider.GetRequiredService<RowVersionSaveChangesInterceptor>());
        });

        return services;
    }
}
