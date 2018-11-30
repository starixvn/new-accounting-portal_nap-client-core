namespace Fanex.NAP.Client.Models
{
    using System;

    [Serializable]
    public class Function
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }

        public Function Parent { get; set; }

        public short DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public byte Crud { get; set; }

        public string Environment { get; set; }

        public Site Site { get; set; }

        public User Creator { get; set; }

        public DateTime CreatedDate { get; set; }

        public User Modifier { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}