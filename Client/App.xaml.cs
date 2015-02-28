using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.ServiceModel;
using System.Globalization;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(ExceptionHandler);

            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            ci.DateTimeFormat.ShortTimePattern = "HH:mm";
            Thread.CurrentThread.CurrentCulture = ci;
        }

        void ExceptionHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //ToDo: handle exceptions
        }

        public static void SelectCulture(string culture)
        {
            // List all our resources      
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                if (dictionary.Source != null && dictionary.Source.OriginalString.StartsWith("Strings"))
                    dictionaryList.Add(dictionary);
            }

            // We want our specific culture      
            string requestedCulture = string.Format("Strings\\{0}\\Resources.xaml", culture);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
            if (resourceDictionary == null)
            {
                // If not found, we select our default language            
                requestedCulture = "Strings\\en-US\\Resources.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
            }

            // If we have the requested resource, remove it from the list and place at the end.      
            // Then this language will be our string table to use.      
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            // Inform the threads of the new culture      
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }
    }
}
