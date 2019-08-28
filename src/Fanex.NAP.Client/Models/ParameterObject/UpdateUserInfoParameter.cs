namespace Fanex.NAP.Client.Models.ParameterObject
{
    using System;

    [Serializable]
    public class UpdateUserInfoParameter
    {
        public int CustId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public int GroupId { get; set; }
    }
}