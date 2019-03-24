using DynastyRazer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.Models
{
    public class ChapterListItemModel : ViewModelBase
    {
        private bool _isSelected;

        public string Title { get; set; }
        public string Permalink { get; set; }

        // When it is already locally Saved
        public bool Exists { get; set; }

        // When it is in the Download List
        public bool IsSelected { get; set; }

        public bool IsChecked
        {
            get
            {
                return Exists || IsSelected;
            }
            set
            {
                IsSelected = value;
                OnPropertyChanged();
            }
        }


        public ChapterModel Chapter { get; set; }
    }
}
