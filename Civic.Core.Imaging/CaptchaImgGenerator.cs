using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civic.Core.Imaging
{
    public static class CaptchaImgGenerator
    {

        public static Dictionary<string,RandomImage> GetRandomImage()
        {
            var result = new Dictionary<string, RandomImage>();
            var capText = GenerateRandomCode();
            RandomImage ci = new RandomImage(capText, 300, 75);
            result.Add(capText, ci);
            return result;
        }
        // Function to generate random string with Random class.
        private static string GenerateRandomCode()
        {
            Random r = new Random();
            string s = "";
            for (int j = 0; j < 5; j++)
            {
                int i = r.Next(3);
                int ch;
                switch (i)
                {
                    case 1:
                        ch = r.Next(0, 9);
                        s = s + ch.ToString();
                        break;
                    case 2:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    case 3:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    default:
                        ch = r.Next(97, 122);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                }
                r.NextDouble();
                r.Next(100, 1999);
            }
            return s;
        }

    }
}
