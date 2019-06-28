using DynastyRazer.Models;
using DynastyRazer.Services;
using DynastyRazer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            _mangaProviderService = 
                new DynastyScansService(new MangaProviderConfiguration());
            DataContext = new MainViewModel(_mangaProviderService);
        }


        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow win = new AboutWindow();
            win.Show();
        }

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is SettingsViewModel))
                DataContext = new SettingsViewModel(_mangaProviderService);
        }

        private void MenuItemStarts_Click(object sender, RoutedEventArgs e)
        {
            if(!(DataContext is MainViewModel))
                DataContext = new MainViewModel(_mangaProviderService);
        }
    }
}
