using MoviesAPI.CQS;
using MoviesAPI.Utilities;
using Polly;
using Polly.Registry;

namespace MoviesAPI.CommandHandlerDecorators
{
    public class ResilientCommandHandlerAsyncDecorator<TCommand> : ICommandHandlerAsync<TCommand> where TCommand : ICommand
    {
        private readonly IAsyncPolicy _resilientSqlAsyncPolicy;
        private readonly ICommandHandlerAsync<TCommand> _decoratee;

        public ResilientCommandHandlerAsyncDecorator(
            IReadOnlyPolicyRegistry<string> policyRegistry,
            ICommandHandlerAsync<TCommand> decoratee)
        {
            if (policyRegistry == null)
                throw new ArgumentNullException(nameof(policyRegistry));
            if (!policyRegistry.TryGet(PollyPolicies.SqlResiliencePolicy, out _resilientSqlAsyncPolicy))
                throw new ArgumentException("Missing Polly policy.");
            
            _decoratee = decoratee;
        }

        public async Task HandleAsync(TCommand command)
        {
            await _resilientSqlAsyncPolicy
                .ExecuteAsync(() => _decoratee.HandleAsync(command))
                .ConfigureAwait(false);
        }
    }
}
