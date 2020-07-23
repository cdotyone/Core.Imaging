using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Imaging
{
    public static class CaptchaImgGenerator
    {
        //Note: This will never use L, I, 1, 0, or O because the resulting image is confusing for end users
        private const string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";

        public static Dictionary<string,RandomImage> GetRandomImage()
        {
            var result = new Dictionary<string, RandomImage>();
            var capText = GenerateRandomCode(5);
            RandomImage ci = new RandomImage(capText, 300, 75);
            result.Add(capText, ci);
            return result;
        }
        // Function to generate random string with Random class.
        private static string GenerateRandomCode(int length)
        {
            Random r = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[r.Next(s.Length)]).ToArray());
        }

    }
}
