using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DynastyRazer.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OpenWindow<T>(ref Window window) where T : Window, new()
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w is T)
                {
                    w.Activate();
                    return;
                }
            }

            window = new T();
            window.Show();
        }

        protected void CloseWindow(Window window)
        {
            window?.Close();
        }



    }
}
