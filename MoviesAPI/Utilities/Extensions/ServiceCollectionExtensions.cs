using Microsoft.Data.SqlClient;
using MoviesApi.Domain.CommandHandlerDecorators;
using MoviesApi.Domain.Commands;
using MoviesApi.EntityFramework;
using MoviesApi.EntityFramework.CommandHandlers;
using MoviesAPI.CQS;
using Polly;
using Polly.Registry;

namespace MoviesAPI.Utilities.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static void ConfigureCQS(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandlerAsync<CreateMovieCommand>>(s =>
            {
                var logger = s.GetRequiredService<ILogger<LoggingCommandHandlerAsyncDecorator<CreateMovieCommand>>>();
                var context = s.GetRequiredService<MoviesContext>();

                var createMovieCommandHandlerAsync = new CreateMovieCommandHandlerAsync(context);
                return new LoggingCommandHandlerAsyncDecorator<CreateMovieCommand>(logger, createMovieCommandHandlerAsync);
            });
        }

        internal static void ConfigurePollyPolicies(this IServiceCollection services)
        {
            var registry = new PolicyRegistry
            {
                { PollyPolicies.SqlResiliencePolicy, Policy.Handle<SqlException>().CircuitBreakerAsync(2, TimeSpan.FromMinutes(2)) }
            };

            services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);
        }
    }
}
