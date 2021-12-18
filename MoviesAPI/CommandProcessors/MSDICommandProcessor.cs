using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MoviesAPI.CQS;

namespace MoviesAPI.CommandProcessors
{
    [SuppressMessage(
        "Performance",
        "CA1812: Avoid uninstantiated internal classes", 
        Justification = "Instantiated via DI container.")]
    internal sealed class MSDICommandProcessor : ICommandProcessor
    {
        private readonly IServiceScope _serviceProvider;

        public MSDICommandProcessor(IServiceScope serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [DebuggerStepThrough]
        public void Process(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>)
               .MakeGenericType(command.GetType());

            dynamic handler = _serviceProvider.ServiceProvider.GetRequiredService(handlerType);

            handler.Handle((dynamic)command);
        }
    }
}
