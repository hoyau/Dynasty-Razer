using System.Globalization;
using System.Threading;
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
