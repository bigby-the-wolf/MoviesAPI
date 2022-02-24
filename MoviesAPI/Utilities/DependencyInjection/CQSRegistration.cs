using MoviesApi.Domain.Commands;
using MoviesApi.EntityFramework.CommandHandlers;
using MoviesAPI.CQS;

namespace MoviesAPI.Utilities.DependencyInjection
{
    internal static class CQSRegistration
    {
        internal static void RegisterCQS(IServiceCollection services)
        {
            services.AddScoped<ICommandHandlerAsync<CreateMovieCommand>, CreateMovieCommandHandlerAsync>();
        }
    }
}
