namespace MoviesAPI.CQS
{
    public interface IQueryHandlerAsync<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}
