namespace MoviesAPI.CQS
{
    public interface ICommandProcessor
    {
        void Process(ICommand command);
    }
}
