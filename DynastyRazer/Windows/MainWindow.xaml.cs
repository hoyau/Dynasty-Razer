using DynastyRazer.Services;
using DynastyRazer.ViewModels;
using System.Windows;

namespace DynastyRazer.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMangaProviderService _mangaProviderService;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _mangaProviderService = new DynastyScansService();
            DataContext = new MainViewModel(_mangaProviderService);
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow();
            window.Show();
        }

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsViewModel))
                DataContext = new SettingsViewModel(_mangaProviderService);
        }

        private void MenuItemStarts_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is MainViewModel))
                DataContext = new MainViewModel(_mangaProviderService);
        }
    }
}
