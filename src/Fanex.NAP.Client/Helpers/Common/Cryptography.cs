//
namespace Fanex.NAP.Client.Helpers.Common
{
    using System;

    public static class Cryptography
    {
        public static string CfsEnCode(string codeStr)
        {
            var num = 30;
            var num2 = num - codeStr.Length;

            if (num2 > 1)
            {
                for (int m = 1; m <= num2; m++)
                {
                    codeStr += Convert.ToChar(0x15);
                }
            }

            var num3 = 1.0;

            for (int i = 1; i <= num; i++)
            {
                if (num <= codeStr.Length)
                {
                    var result = Convert.ToChar(codeStr.Substring(i - 1, 1));

                    int num5 = num + (result * i);
                    num3 *= num5;
                }
            }

            codeStr = num3.ToString();
            string str = string.Empty;
            for (int j = 1; j <= codeStr.Length; j++)
            {
                var length = 3;

                if (codeStr.Length - j < 2)
                {
                    length = codeStr.Length - j + 1;
                }

                str += CfsCode(codeStr.Substring(j - 1, length));
            }

            string str2 = string.Empty;
            for (int k = 20; k <= (str.Length - 0x12); k += 2)
            {
                str2 += str.Substring(k - 1, 1);
            }

            return str2;
        }

        private static string CfsCode(string value)
        {
            string number = string.Empty;

            for (int i = 1; i <= value.Length; i++)
            {
                number += (int)Convert.ToChar(value.Substring(i - 1, 1));
            }

            return (Convert.ToInt32(number)).ToString("X");
        }
    }
}