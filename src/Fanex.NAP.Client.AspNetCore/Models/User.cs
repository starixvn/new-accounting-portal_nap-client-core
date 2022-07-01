namespace Fanex.NAP.Client.Models
{
    using System;

    [Serializable]
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int GroupId { get; set; }
    }
}