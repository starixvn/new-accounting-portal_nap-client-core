namespace Fanex.NAP.Client.Models.Command
{
    public class CommandResult : CommandResultBase
    {
        public override bool IsSuccessful => Code == 0;
    }
}