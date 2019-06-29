using DynastyRazer.Commands;
using DynastyRazer.Models;
using DynastyRazer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DynastyRazer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IMangaProviderService _service;
        private List<SerieListItem> _series;
        private List<SerieListItem> _filteredSeries;
        private SerieDetails _serieDetails;
        private ObservableCollection<ChapterListItem> _chaptersToDownload;
        private SerieListItem _selectedSerie;
        private bool _isDownloading;
        private bool _isSelectAllChaptersChecked;
        private string _downloadStatusText;
        private int _pagesToDownloadCount;
        private int _pagesDownloadedCount;

        private ICommand _downloadClick;
        private ICommand _selectAllChaptersChange;
        private ICommand _chapterClick;
        private ICommand _mangaFilterChanged;

        public MainViewModel(IMangaProviderService service)
        {
            Task.Run(async () =>
            {
                _service = service;
                Series = await _service.RetrieveAllSeries();
                FilteredSeries = new List<SerieListItem>(Series);
                ChaptersToDownload = new ObservableCollection<ChapterListItem>();

                IsDownloading = false;
                IsSelectAllChaptersChecked = false;

                ResetProgressBarDatas();

                SelectAllChaptersChange = new RelayCommand<CheckBox>(OnSelectAllChaptersChange, CanExecuteSelectAllChaptersChange);
                ChapterClick = new RelayCommand<CheckBox>(OnChapterClick);
                MangaFilterChanged = new RelayCommand<string>(OnMangaFilterChanged);
                DownloadClick = new RelayCommand<object>(async (p) => await OnDownloadClick(), CanExecuteDownloadClick);
                _service.PageDownloadStateChanged += OnPageDownloadStateChanged;
            });
        }

        public bool IsSelectAllChaptersChecked
        {
            get => _isSelectAllChaptersChecked;
            set { _isSelectAllChaptersChecked = value; NotifyPropertyChanged(); }
        }

        public List<SerieListItem> Series
        {
            get => _series;
            set { _series = value; NotifyPropertyChanged(); }
        }

        public string DownloadStatusText
        {
            get => _downloadStatusText;
            set { _downloadStatusText = value; NotifyPropertyChanged(); }
        }

        public List<SerieListItem> FilteredSeries
        {
            get { return _filteredSeries; }
            set { _filteredSeries = value; NotifyPropertyChanged(); }
        }

        public SerieDetails SerieDetails
        {
            get => _serieDetails;
            set
            {
                _serieDetails = value;
                if (_serieDetails == null)
                    return;

                foreach (ChapterListItem chapter in _serieDetails.Taggings)
                    chapter.IsSelected = ChaptersToDownload.Where(x => x.Permalink.Equals(chapter.Permalink)).Count() > 0;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ChapterListItem> ChaptersToDownload
        {
            get => _chaptersToDownload; 
            set { _chaptersToDownload = value; NotifyPropertyChanged(); }
        }

        public SerieListItem SelectedSerie
        {
            get => _selectedSerie; 
            set
            {
                _selectedSerie = value;
                NotifyPropertyChanged();
                _ = LoadSerieDetails();
            }
        }

        public bool IsDownloading
        {
            get => _isDownloading;
            set { _isDownloading = value; NotifyPropertyChanged(); }
        }

        public int PagesToDownloadCount
        {
            get => _pagesToDownloadCount;
            set { _pagesToDownloadCount = value; NotifyPropertyChanged(); }
        }

        public int PagesDownloadedCount
        {
            get => _pagesDownloadedCount;
            set { _pagesDownloadedCount = value; NotifyPropertyChanged(); }
        }

        public ICommand ChapterClick
        {
            get => _chapterClick;
            set { _chapterClick = value; NotifyPropertyChanged(); }
        }

        public ICommand SelectAllChaptersChange
        {
            get => _selectAllChaptersChange;
            set { _selectAllChaptersChange = value; NotifyPropertyChanged(); }
        }

        public ICommand DownloadClick
        {
            get => _downloadClick;
            set { _downloadClick = value; NotifyPropertyChanged(); }
        }

        public ICommand MangaFilterChanged
        {
            get => _mangaFilterChanged;
            set { _mangaFilterChanged = value; NotifyPropertyChanged(); }
        }

        private async Task LoadSerieDetails()
        {
            SerieDetails = await _service.RetrieveSerieDetails(_selectedSerie);
        }

        private bool CanExecuteDownloadClick(object param)
        {
            return !_isDownloading;
        }

        private void InitProgressBarDatas()
        {
            int pageCount = 0;
            foreach (var item in ChaptersToDownload)
                pageCount += item.Chapter.Pages.Count;
            PagesToDownloadCount = pageCount;
            PagesDownloadedCount = 0;
        }

        private void ResetProgressBarDatas()
        {
            PagesToDownloadCount = 1;
            PagesDownloadedCount = 0;
        }

        private bool CanExecuteSelectAllChaptersChange(object param)
        {
            return _serieDetails != null && !_isDownloading;
        }

        private void OnChapterClick(CheckBox c)
        {
            bool isChecked = (bool)c.IsChecked;
            ChapterListItem chapterItem = (ChapterListItem)c.DataContext;
            chapterItem.IsSelected = isChecked;

            if (isChecked)
                ChaptersToDownload.Add(chapterItem);
            else
                ChaptersToDownload.Remove(chapterItem);
        }

        private void OnSelectAllChaptersChange(CheckBox c)
        {
            bool isChecked = (bool)c.IsChecked;

            foreach (ChapterListItem chapterItem in _serieDetails.Taggings)
            {
                if (!chapterItem.IsLocallySaved)
                {
                    chapterItem.IsChecked = isChecked;
                    if (isChecked && !ChaptersToDownload.Contains(chapterItem))
                        ChaptersToDownload.Add(chapterItem);
                    else if (!isChecked && ChaptersToDownload.Contains(chapterItem))
                        ChaptersToDownload.Remove(chapterItem);
                }
            }

        }

        private async Task OnDownloadClick()
        {
            await Task.Run(() =>
            {
                IsDownloading = true;
                _service.AssignChapter(ChaptersToDownload.ToList());
                InitProgressBarDatas();

                foreach (var item in ChaptersToDownload)
                {
                    foreach (var page in item.Chapter.Pages)
                    {
                        _service.DownloadPage(page, item.Chapter.Tags.Where(x => x.Type.Equals("Series")).First().Name, item.Title);
                        PagesDownloadedCount++;
                    }
                }

                ChaptersToDownload.Clear();
                _ = LoadSerieDetails();

                IsSelectAllChaptersChecked = false;
                IsDownloading = false;
            });
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnMangaFilterChanged(string filterString)
        {
            IEnumerable<SerieListItem> t = from serie in Series
                                                where serie.Name.ToLower().IndexOf(filterString.ToLower()) >= 0
                                                select serie;
            FilteredSeries = t.ToList();
        }

        private void OnPageDownloadStateChanged(object param, string s)
        {
            DownloadStatusText = s;
        }





    }
}
