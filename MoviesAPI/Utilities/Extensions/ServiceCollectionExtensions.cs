using Microsoft.Data.SqlClient;
using MoviesApi.Domain.CommandHandlerDecorators;
using MoviesApi.Domain.QueryHandlerDecorators;
using MoviesApi.EntityFramework;
using MoviesApi.EntityFramework.CommandHandlers;
using MoviesApi.EntityFramework.QueryHandlers;
using MoviesAPI.CommandHandlerDecorators;
using MoviesAPI.CQS;
using MoviesAPI.QueryHandlerDecorators;
using Polly;
using Polly.Registry;

namespace MoviesAPI.Utilities.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static void ConfigurePollyPolicies(this IServiceCollection services)
        {
            var registry = new PolicyRegistry
            {
                { PollyPolicies.SqlResiliencePolicy, Policy.Handle<SqlException>().CircuitBreakerAsync(2, TimeSpan.FromMinutes(2)) }
            };

            services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);
        }

        internal static void ConfigureCQS(this IServiceCollection services)
        {
            services.AddScoped(s =>
            {
                var context = s.GetRequiredService<MoviesContext>();

                var createMovieCommandHandlerAsync = new CreateMovieCommandHandlerAsync(context);
                
                return createMovieCommandHandlerAsync.AddLoggingDecorator(s).AddResilientDecorator(s);
            });

            services.AddScoped(s =>
            {
                var context = s.GetRequiredService<MoviesContext>();

                var getAllMoviesQueryHandlerAsync = new GetAllMoviesQueryHandlerAsync(context);

                return getAllMoviesQueryHandlerAsync.AddLoggingDecorator(s).AddResilientDecorator(s);
            });

            services.AddScoped(s =>
            {
                var context = s.GetRequiredService<MoviesContext>();

                var getMovieByIdQueryHandlerAsync = new GetMovieByIdQueryHandlerAsync(context);

                return getMovieByIdQueryHandlerAsync.AddLoggingDecorator(s).AddResilientDecorator(s);
            });
        }

        private static ICommandHandlerAsync<TCommand> AddLoggingDecorator<TCommand>(
            this ICommandHandlerAsync<TCommand> decoratee,
            IServiceProvider serviceProvider) where TCommand : ICommand
        {
            var logger = serviceProvider
                .GetRequiredService<ILogger<LoggingCommandHandlerAsyncDecorator<TCommand>>>();

            return new LoggingCommandHandlerAsyncDecorator<TCommand>(logger, decoratee);
        }

        private static ICommandHandlerAsync<TCommand> AddResilientDecorator<TCommand>(
            this ICommandHandlerAsync<TCommand> decoratee,
            IServiceProvider serviceProvider) where TCommand : ICommand
        {
            var policyRegistry = serviceProvider
                .GetRequiredService<IReadOnlyPolicyRegistry<string>>();

            return new ResilientCommandHandlerAsyncDecorator<TCommand>(policyRegistry, decoratee);
        }

        private static IQueryHandlerAsync<TQuery, TResult> AddLoggingDecorator<TQuery, TResult>(
            this IQueryHandlerAsync<TQuery, TResult> decoratee,
            IServiceProvider serviceProvider) where TQuery : IQuery<TResult>
        {
            var logger = serviceProvider
                .GetRequiredService<ILogger<LoggingQueryHandlerAsyncDecorator<TQuery, TResult>>>();

            return new LoggingQueryHandlerAsyncDecorator<TQuery, TResult>(logger, decoratee);
        }

        private static IQueryHandlerAsync<TQuery, TResult> AddResilientDecorator<TQuery, TResult>(
            this IQueryHandlerAsync<TQuery, TResult> decoratee,
            IServiceProvider serviceProvider) where TQuery : IQuery<TResult>
        {
            var policyRegistry = serviceProvider
                .GetRequiredService<IReadOnlyPolicyRegistry<string>>();

            return new ResilientQueryHandlerAsyncDecorator<TQuery, TResult>(policyRegistry, decoratee);
        }
    }
}
