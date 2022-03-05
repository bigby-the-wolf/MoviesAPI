using MoviesAPI.CQS;
using MoviesAPI.Utilities;
using Polly;
using Polly.Registry;

namespace MoviesAPI.QueryHandlerDecorators
{
    public class ResilientQueryHandlerAsyncDecorator<TQuery, TResult> : IQueryHandlerAsync<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly IAsyncPolicy _resilientSqlAsyncPolicy;
        private readonly IQueryHandlerAsync<TQuery, TResult> _decoratee;

        public ResilientQueryHandlerAsyncDecorator(
            IReadOnlyPolicyRegistry<string> policyRegistry,
            IQueryHandlerAsync<TQuery, TResult> decoratee)
        {
            if (policyRegistry == null)
                throw new ArgumentNullException(nameof(policyRegistry));
            if (!policyRegistry.TryGet(PollyPolicies.SqlResiliencePolicy, out _resilientSqlAsyncPolicy))
                throw new ArgumentException("Missing Polly policy.");
            
            _decoratee = decoratee;
        }

        public async Task<TResult> HandleAsync(TQuery query)
        {
            return await _resilientSqlAsyncPolicy
                .ExecuteAsync(() => _decoratee.HandleAsync(query))
                .ConfigureAwait(false);
        }
    }
}
