using Microsoft.Extensions.Logging;
using MoviesAPI.CQS;
using System.ComponentModel;
using System.Text.Json;

namespace MoviesApi.Domain.QueryHandlerDecorators
{
    public class LoggingQueryHandlerAsyncDecorator<TQuery, TResult> : IQueryHandlerAsync<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly ILogger<LoggingQueryHandlerAsyncDecorator<TQuery, TResult>> _logger;
        private readonly IQueryHandlerAsync<TQuery, TResult> _decoratee;

        public LoggingQueryHandlerAsyncDecorator(
            ILogger<LoggingQueryHandlerAsyncDecorator<TQuery, TResult>> logger,
            IQueryHandlerAsync<TQuery, TResult> decoratee)
        {
            _logger = logger;
            _decoratee = decoratee;
        }

        public async Task<TResult> HandleAsync(TQuery query)
        {
            var result = await _decoratee
                .HandleAsync(query)
                .ConfigureAwait(false);

            var serializedResult = SerializeResult(result);

            _logger.LogInformation(
                "Query: {queryName}, Value: {queryValue}, Result: {queryResult}",
                TypeDescriptor.GetClassName(query),
                JsonSerializer.Serialize(query),
                serializedResult);

            return result;
        }

        private static string SerializeResult(TResult result)
        {
            string? serializedResult;
            
            try
            {
                serializedResult = JsonSerializer.Serialize(result);
            }
            catch
            {
                serializedResult = result == null ? "Result is null" : result.ToString();
            }

            return serializedResult!;
        }
    }
}
