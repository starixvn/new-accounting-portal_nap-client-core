namespace Fanex.Nap.Client.AspNetCore.DemoSite.Controllers
{
using System.Security.Cryptography;
using System.Text;
using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [AllowAnonymous]
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            var content = SHA256("1234567890");

            return Content(content);
        }

        private string SHA256(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Hey guy! I dont want to hash an empty or white space string");
            }

            var hashAlgorithm = new SHA256Managed();
            var hashData = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            string encrypted = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLower();

            return encrypted;
        }
    }
}