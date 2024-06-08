using System.Text.RegularExpressions;
using Convert = System.Convert;

namespace ConsoleApp1
{
    public static class TextUtilities
    {
        public static decimal GetNumber(this string input)
        {
            var number = Regex.Replace(input, "[^0-9-.]", "").Trim();
            return string.IsNullOrEmpty(number) ? 0 : Convert.ToDecimal(number);
        }
    }
}
