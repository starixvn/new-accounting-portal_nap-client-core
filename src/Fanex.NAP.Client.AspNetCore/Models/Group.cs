namespace Fanex.NAP.Client.Models
{
    using System;

    [Serializable]
    public class Group
    {
        public short Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public User Creator { get; set; }

        public DateTime CreatedDate { get; set; }

        public User Modifier { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int NumOfUser { get; set; }
    }
}