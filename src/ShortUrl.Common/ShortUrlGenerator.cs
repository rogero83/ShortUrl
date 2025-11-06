using System.Text.RegularExpressions;

namespace ShortUrl.Common
{
    public static class ShortUrlGenerator
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private static readonly Regex _validPattern = new(@"^[a-zA-Z0-9_-]{4,199}$", RegexOptions.Compiled);

        public static string GenerateShortCode(int length = 7)
        {
            var shortCode = new char[length];
            for (int i = 0; i < length; i++)
            {
                shortCode[i] = Alphabet[Random.Shared.Next(Alphabet.Length)];
            }
            return new string(shortCode);
        }

        public static bool IsValidShortCode(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode))
                return false;

            return _validPattern.IsMatch(shortCode);
        }
    }
}
