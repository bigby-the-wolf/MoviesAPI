using CSharpFunctionalExtensions;
using Microsoft.Data.SqlClient;
using MoviesApi.Domain.CommandHandlerDecorators;
using MoviesApi.Domain.Commands;
using MoviesApi.Domain.Entities;
using MoviesApi.Domain.Queries;
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
        internal static void ConfigureCQS(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandlerAsync<CreateMovieCommand>>(s =>
            {
                var policyRegistry = s.GetRequiredService<IReadOnlyPolicyRegistry<string>>();
                var logger = s.GetRequiredService<ILogger<LoggingCommandHandlerAsyncDecorator<CreateMovieCommand>>>();
                var context = s.GetRequiredService<MoviesContext>();

                var createMovieCommandHandlerAsync = new CreateMovieCommandHandlerAsync(context);
                var loggingDecorator = new LoggingCommandHandlerAsyncDecorator<CreateMovieCommand>(logger, createMovieCommandHandlerAsync);
                
                return new ResilientCommandHandlerAsyncDecorator<CreateMovieCommand>(policyRegistry, loggingDecorator);
            });

            services.AddScoped<IQueryHandlerAsync<GetAllMoviesQuery, IReadOnlyCollection<Movie>>>(s =>
            {
                var policyRegistry = s.GetRequiredService<IReadOnlyPolicyRegistry<string>>();
                var logger = s.GetRequiredService<ILogger<LoggingQueryHandlerAsyncDecorator<GetAllMoviesQuery, IReadOnlyCollection<Movie>>>>();
                var context = s.GetRequiredService<MoviesContext>();

                var getAllMoviesQueryHandlerAsync = new GetAllMoviesQueryHandlerAsync(context);
                var loggingDecorator = new LoggingQueryHandlerAsyncDecorator<GetAllMoviesQuery, IReadOnlyCollection<Movie>>(logger, getAllMoviesQueryHandlerAsync);

                return new ResilientQueryHandlerAsyncDecorator<GetAllMoviesQuery, IReadOnlyCollection<Movie>>(policyRegistry, loggingDecorator);
            });

            services.AddScoped<IQueryHandlerAsync<GetMovieByIdQuery, Maybe<Movie>>>(s =>
            {
                var policyRegistry = s.GetRequiredService<IReadOnlyPolicyRegistry<string>>();
                var logger = s.GetRequiredService<ILogger<LoggingQueryHandlerAsyncDecorator<GetMovieByIdQuery, Maybe<Movie>>>>();
                var context = s.GetRequiredService<MoviesContext>();

                var getMovieByIdQueryHandlerAsync = new GetMovieByIdQueryHandlerAsync(context);
                var loggingDecorator = new LoggingQueryHandlerAsyncDecorator<GetMovieByIdQuery, Maybe<Movie>>(logger, getMovieByIdQueryHandlerAsync);

                return new ResilientQueryHandlerAsyncDecorator<GetMovieByIdQuery, Maybe<Movie>>(policyRegistry, loggingDecorator);
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
