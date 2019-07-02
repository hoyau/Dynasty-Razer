using DynastyRazer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.Models
{
    public class ChapterListItem : ViewModelBase
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsLocallySaved { get; set; }
        public bool IsSelected { get; set; }
        public bool IsChecked
        {
            get => IsLocallySaved || IsSelected;
            set { IsSelected = value; NotifyPropertyChanged(); }
        }
        public string MangaTitle { get; set; }

        /// <summary>
        /// Lazy Load Property
        /// </summary>
        public ChapterDetails ChapterDetails { get; set; }

    }
}
