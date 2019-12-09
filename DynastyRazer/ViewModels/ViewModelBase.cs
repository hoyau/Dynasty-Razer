using System.ComponentModel;
using System.Runtime.CompilerServices;
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
            // if the window is already open, activate it
            foreach (Window existingWindow in Application.Current.Windows)
            {
                if (!(existingWindow is T))
                    continue;

                existingWindow.Activate();
                return;
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
