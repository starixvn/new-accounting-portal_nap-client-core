namespace Fanex.NAP.Client.Models.Command
{
    public class CommandResultBase : ICommandResult
    {
        public CommandResultBase()
            : this(0, string.Empty)
        {
        }

        public CommandResultBase(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public virtual bool IsSuccessful => true;
    }
}