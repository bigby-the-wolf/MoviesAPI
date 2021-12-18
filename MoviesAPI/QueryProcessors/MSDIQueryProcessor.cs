using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MoviesAPI.CQS;

namespace MoviesAPI.QueryProcessors
{
    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes", 
        Justification = "Instantiated via DI container.")]
    internal sealed class MSDIQueryProcessor : IQueryProcessor
    {
        private readonly IServiceScope _serviceProvider;

        public MSDIQueryProcessor(IServiceScope serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [DebuggerStepThrough]
        public TResult Process<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>)
                .MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = _serviceProvider.ServiceProvider.GetRequiredService(handlerType);

            return handler.Handle((dynamic)query);
        }
    }
}
