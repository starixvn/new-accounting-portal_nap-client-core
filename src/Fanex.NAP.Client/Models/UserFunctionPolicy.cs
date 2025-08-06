namespace Fanex.NAP.Client.Models
{
    using System;

    [Serializable]
    public class UserFunctionPolicy
    {
        public int UserId { get; set; }
        public int FunctionId { get; set; }
        public int Policy { get; set; }
    }
}