using DynastyRazer.Commands;
using DynastyRazer.Models;
using DynastyRazer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        private string _serieImageUrl;

        private ICommand _downloadClickCommand;
        private ICommand _chaptersSelectAllChangeCommand;
        private ICommand _chapterClickCommand;
        private ICommand _mangaFilterChangedCommand;

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

                ChaptersSelectAllChangeCommand = new RelayCommand<CheckBox>(ChaptersSelectAllChange, CanExecuteSelectAllChaptersChange);
                ChapterClickCommand = new RelayCommand<CheckBox>(ChapterClick);
                MangaFilterChangedCommand = new RelayCommand<string>(MangaFilterChanged);
                DownloadClickCommand = new RelayCommand<object>(async (p) => await DownloadClick(), CanExecuteDownloadClick);
                _service.PageDownloadStateChanged += PageDownloadStateChanged;
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

        public string SerieImageUrl
        {
            get => _serieImageUrl;
            set { _serieImageUrl = value; NotifyPropertyChanged(); }
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

                foreach (var chapter in _serieDetails.Chapters)
                {
                    chapter.IsSelected = ChaptersToDownload.Any(x => x.Url.Equals(chapter.Url));
                }

                SerieImageUrl = _serieDetails.ImageUrl;
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

        public ICommand ChapterClickCommand
        {
            get => _chapterClickCommand;
            set { _chapterClickCommand = value; NotifyPropertyChanged(); }
        }

        public ICommand ChaptersSelectAllChangeCommand
        {
            get => _chaptersSelectAllChangeCommand;
            set { _chaptersSelectAllChangeCommand = value; NotifyPropertyChanged(); }
        }

        public ICommand DownloadClickCommand
        {
            get => _downloadClickCommand;
            set { _downloadClickCommand = value; NotifyPropertyChanged(); }
        }

        public ICommand MangaFilterChangedCommand
        {
            get => _mangaFilterChangedCommand;
            set { _mangaFilterChangedCommand = value; NotifyPropertyChanged(); }
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
            var pageCount = 0;

            foreach (var item in ChaptersToDownload)
            {
                pageCount += item.ChapterDetails.Pages.Count;
            }

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

        private void ChapterClick(CheckBox c)
        {
            if (c?.DataContext == null)
                return;

            var isChecked = (bool)c.IsChecked;
            var chapterItem = (ChapterListItem)c.DataContext;
            chapterItem.IsSelected = isChecked;

            if (isChecked)
                ChaptersToDownload.Add(chapterItem);
            else
                ChaptersToDownload.Remove(chapterItem);
        }

        private void ChaptersSelectAllChange(CheckBox c)
        {
            var isChecked = (bool)c.IsChecked;

            if (_serieDetails?.Chapters == null)
                return;

            foreach (var chapterItem in _serieDetails.Chapters)
            {
                if (chapterItem.IsLocallySaved)
                    continue;

                chapterItem.IsChecked = isChecked;
                if (isChecked && !ChaptersToDownload.Contains(chapterItem))
                    ChaptersToDownload.Add(chapterItem);
                else if (!isChecked && ChaptersToDownload.Contains(chapterItem))
                    ChaptersToDownload.Remove(chapterItem);
            }
        }

        private async Task DownloadClick()
        {
            await Task.Run(async () =>
            {
                try
                {
                    IsDownloading = true;
                    await _service.AssignChapterDetails(ChaptersToDownload.ToList());
                    InitProgressBarDatas();

                    await _service.StartDownload(ChaptersToDownload.ToList());

                    DownloadStatusText = ChaptersToDownload.Count > 0 ? "Success" : "";
                    ChaptersToDownload.Clear();
                    _ = LoadSerieDetails();

                    IsSelectAllChaptersChecked = false;
                    IsDownloading = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            CommandManager.InvalidateRequerySuggested();
        }

        private void MangaFilterChanged(string filterString)
        {
            var filteredSeries = from serie in Series
                                 where serie.Title.ToLower().IndexOf(filterString.ToLower()) >= 0
                                 select serie;
            FilteredSeries = filteredSeries.ToList();
        }

        private void PageDownloadStateChanged(object param, string s)
        {
            DownloadStatusText = s;
            PagesDownloadedCount++;
        }
    }
}
