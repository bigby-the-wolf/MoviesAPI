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
        private readonly IServiceProvider _serviceProvider;

        public MSDICommandProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [DebuggerStepThrough]
        public void Process(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>)
               .MakeGenericType(command.GetType());

            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            handler.Handle((dynamic)command);
        }
    }
}
