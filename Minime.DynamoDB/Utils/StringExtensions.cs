using System.Text.RegularExpressions;

namespace Minime.DynamoDB.Utils
{
    public static class StringExtensions
    {
        private static readonly Random _random = new Random();
        private const int _lenght = 6;

        public static string GetStringId(this string term)
            => new(Enumerable.Range(0, _lenght)
            .Select(_ => term[_random.Next(term.Length)])
            .ToArray());

        public static string OnlyLettersAndNumbers(this string term)
          => Regex.Replace(term, "[^a-zA-Z0-9]", "");
    }
}
