namespace Fanex.NAP.Client.Models.Command
{
    public interface ICommandResult
    {
        int Code { get; }

        string Message { get; }

        bool IsSuccessful { get; }
    }
}