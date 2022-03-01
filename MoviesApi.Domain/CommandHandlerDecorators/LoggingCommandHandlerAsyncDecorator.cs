using Microsoft.Extensions.Logging;
using MoviesAPI.CQS;
using System.ComponentModel;
using System.Text.Json;

namespace MoviesApi.Domain.CommandHandlerDecorators
{
    public class LoggingCommandHandlerAsyncDecorator<TCommand> : ICommandHandlerAsync<TCommand> where TCommand : ICommand
    {
        private readonly ILogger<LoggingCommandHandlerAsyncDecorator<TCommand>> _logger;
        private readonly ICommandHandlerAsync<TCommand> _decoratee;

        public LoggingCommandHandlerAsyncDecorator(
            ILogger<LoggingCommandHandlerAsyncDecorator<TCommand>> logger,
            ICommandHandlerAsync<TCommand> decoratee)
        {
            _logger = logger;
            _decoratee = decoratee;
        }

        public async Task HandleAsync(TCommand command)
        {
            _logger.LogInformation(
                "Command: {commandName}, Value: {commandValue}",
                TypeDescriptor.GetClassName(command),
                JsonSerializer.Serialize(command));

            await _decoratee.HandleAsync(command);
        }
    }
}
