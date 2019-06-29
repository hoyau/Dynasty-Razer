using DynastyRazer.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DynastyRazer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SetLanguage("de-DE");
            base.OnStartup(e);
        }

        public void SetLanguage(string langCode)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(langCode);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCode);
        }

    }
}
