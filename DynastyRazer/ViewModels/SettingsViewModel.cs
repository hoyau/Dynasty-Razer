using DynastyRazer.Commands;
using DynastyRazer.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DynastyRazer.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private string _savePath;
        private ICommand _pathClickCommand;

        public SettingsViewModel(IMangaProviderService service)
        {
            SavePath = StorageService.SavePath;
            PathClickCommand = new RelayCommand<object>((p) => PathClick());
        }

        public ICommand PathClickCommand
        {
            get => _pathClickCommand;
            set { _pathClickCommand = value; NotifyPropertyChanged(); }
        }
        public string SavePath
        {
            get { return _savePath; }
            set { _savePath = value; NotifyPropertyChanged(); }
        }

        private void PathClick()
        {
            var initialDirectory = string.IsNullOrEmpty(SavePath) ? AppDomain.CurrentDomain.BaseDirectory : SavePath;

            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = initialDirectory;
                dialog.IsFolderPicker = true;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    bool result = UpdatePath(dialog.FileName);
                    if (!result)
                        MessageBox.Show("Saving File Path failed");
                }
            }
        }

        private bool UpdatePath(string path)
        {
            var result = false;
            if (!String.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                StorageService.SavePath = path;
                SavePath = StorageService.SavePath;
                result = true;
            }

            return result;
        }
    }
}
