using DynastyRazer.Commands;
using DynastyRazer.Models;
using DynastyRazer.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DynastyRazer.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private IMangaProviderService _service;
        private string _savePath;
        private ICommand _pathClick;

        public SettingsViewModel(IMangaProviderService service)
        {
            _service = service;
            SavePath = _service.Config.SavePath;
            PathClick = new RelayCommand<object>((p) => OnPathClick());
        }

        public string SavePath
        {
            get { return _savePath; }
            set { _savePath = value; OnPropertyChanged(); }
        }

        public ICommand PathClick
        {
            get => _pathClick;
            set { _pathClick = value; OnPropertyChanged(); }
        }

        private  bool UpdatePath(string path)
        {
            bool result = false;
            if (!String.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                _service.Config.SavePath = path;
                SavePath = _service.Config.SavePath;
                result = true;
            }

            return result;
        }

        #region Command Methods
        private void OnPathClick()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = "C:\\Users";
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    bool result = UpdatePath(dialog.FileName);
                    if (!result)
                        MessageBox.Show("Saving File Path failed");
                }
            }
        }
        #endregion

    }
}
