namespace Fanex.NAP.Client.Demo.AspNetCore.Controllers;

using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]
public class TestController : Controller
{
    public IActionResult Index()
    {
        var content = ToSHA256("1234567890");

        return Content(content);
    }

    private static string ToSHA256(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Hey guy! I do not want to hash an empty or white space string");
        }

        using var hashAlgorithm = SHA256.Create();
        var hashData = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        var encrypted = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLower();

        return encrypted;
    }
}