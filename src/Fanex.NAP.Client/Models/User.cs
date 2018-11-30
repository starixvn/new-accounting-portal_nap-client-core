namespace Fanex.NAP.Client.Models
{
    using System;

    [Serializable]
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Group Group { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime LastLoginDate { get; set; }

        public User Creator { get; set; }

        public DateTime CreatedDate { get; set; }

        public User Modifier { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}