using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace OutcomeHub.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}
