namespace Fanex.NAP.Core.Console
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.NAP.Core.Service.Impl;

    public class Program
    {
        public static void Main(string[] args)
        {
            var userPolicies = new UserService().GetUserPolicies("ManageUser", 1);
            Console.ReadLine();
        }

        public static Task<string> DoAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                return "Hello world!";
            });
        }
    }
}