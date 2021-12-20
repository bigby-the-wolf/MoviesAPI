using MoviesApi.EntityFramework;
using MoviesAPI.CommandProcessors;
using MoviesAPI.CQS;
using MoviesAPI.QueryProcessors;

namespace MoviesAPI.Utilities.DependencyInjection
{
    internal static class CQSRegistration
    {
        internal static void RegisterCQS(IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblyOf<MoviesContext>()
                    .AddClasses(classes => classes.AssignableTo(typeof (ICommandHandler<>)))
                        .AsImplementedInterfaces()
                        .WithScopedLifetime()
                    .AddClasses(classes => classes.AssignableTo(typeof (IQueryHandler<,>)))
                        .AsImplementedInterfaces()
                        .WithScopedLifetime());

            services.AddScoped<ICommandProcessor, MSDICommandProcessor>();
            services.AddScoped<IQueryProcessor, MSDIQueryProcessor>();
        }
    }
}
