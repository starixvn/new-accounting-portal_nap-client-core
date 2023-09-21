namespace Fanex.NAP.Client.Models
{
    using System;

    [Serializable]
    public class Group
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}