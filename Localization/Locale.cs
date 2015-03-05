using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Localization
{
    public static class Locale
    {
        private static readonly string[] SupportedLanguages = new string[] { "en-US", "pl-PL" };
        private static Dictionary<string, ResourceDictionary> _localizations;

        public static string CurrentLanguage { get; set; }

        static Locale()
        {
            CurrentLanguage = SupportedLanguages.First();
            InitializeLocalizations();
        }

        private static void InitializeLocalizations()
        {
            _localizations = new Dictionary<string,ResourceDictionary>();
            foreach (var lang in SupportedLanguages)
            {
                _localizations.Add(lang, new ResourceDictionary() { Source = new Uri(String.Format("/Localization;component/Strings/{0}/Resources.xaml", lang), UriKind.RelativeOrAbsolute) });
            }
        }

        public static string GetString(string name)
        {
            return _localizations[CurrentLanguage][name] as string;
        }
    }
}
