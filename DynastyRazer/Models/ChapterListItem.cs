using DynastyRazer.ViewModels;

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
