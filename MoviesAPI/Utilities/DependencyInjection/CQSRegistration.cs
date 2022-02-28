using MoviesApi.Domain.CommandHandlers;
using MoviesApi.Domain.Commands;
using MoviesApi.EntityFramework;
using MoviesApi.EntityFramework.CommandHandlers;
using MoviesAPI.CQS;

namespace MoviesAPI.Utilities.DependencyInjection
{
    internal static class CQSRegistration
    {
        internal static void RegisterCQS(IServiceCollection services)
        {
            services.AddScoped<ICommandHandlerAsync<CreateMovieCommand>>(s =>
            {
                var logger = s.GetRequiredService<ILogger<LoggingCommandHandlerAsyncDecorator<CreateMovieCommand>>>();
                var context = s.GetRequiredService<MoviesContext>();

                var createMovieCommandHandlerAsync = new CreateMovieCommandHandlerAsync(context);
                return new LoggingCommandHandlerAsyncDecorator<CreateMovieCommand>(logger, createMovieCommandHandlerAsync);
            });
        }
    }
}
