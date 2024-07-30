using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Mps.Domain.Extensions
{
    public static class StringExtension
    {
        public static bool SearchIgnoreCase(this string text, string pattern)
        {
            string normalizedText = RemoveDiacritics(text);
            string normalizedPattern = RemoveDiacritics(pattern);
            return Regex.IsMatch(normalizedText, Regex.Escape(normalizedPattern), RegexOptions.IgnoreCase);
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (c == 'đ' || c == 'Đ')
                    {
                        stringBuilder.Append('d');
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
