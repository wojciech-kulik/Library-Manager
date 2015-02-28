using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class StringHelpers
    {
        public static bool ContainsAny(this string text, string words)
        {
            string lowerText = text.ToLower().Trim();
            string[] phrase = words.Trim().ToLower().Split(' ');

            foreach (var word in phrase)
                if (lowerText.Contains(word))
                    return true;

            return false;
        }


        public static string NullIfEmpty(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return null;
            else
                return text;
        }
    }
}
