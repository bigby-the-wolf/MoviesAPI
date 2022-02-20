namespace MoviesAPI.CQS
{
    public interface ICommandHandlerAsync<TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }
}
