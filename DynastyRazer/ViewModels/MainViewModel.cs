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
        #region Init Fields
        private IMangaProviderService _service;
        private List<SerieListItemModel> _series;
        private List<SerieListItemModel> _filteredSeries;
        private SerieDetailsModel _serieDetails;
        private ObservableCollection<ChapterListItemModel> _chaptersToDownload;
        private SerieListItemModel _selectedSerie;
        private bool _isDownloading;
        private bool _isSelectAllChaptersChecked;
        private string _downloadStatusText;
        private int _pagesToDownloadCount;
        private int _pagesDownloadedCount;

        private ICommand _downloadClick;
        private ICommand _selectAllChaptersChange;
        private ICommand _chapterClick;
        private ICommand _mangaFilterChanged;
        #endregion

        public MainViewModel(IMangaProviderService service)
        {
            Task.Run(async () =>
            {
                _service = service;
                Series = await _service.GetAllSeries();
                FilteredSeries = new List<SerieListItemModel>(Series);
                ChaptersToDownload = new ObservableCollection<ChapterListItemModel>();

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

        // GET SET

        #region Get/Set

        public bool IsSelectAllChaptersChecked
        {
            get => _isSelectAllChaptersChecked;
            set { _isSelectAllChaptersChecked = value; OnPropertyChanged(); }
        }

        public List<SerieListItemModel> Series
        {
            get { return _series; }
            set { _series = value; OnPropertyChanged(); }
        }

        public string DownloadStatusText
        {
            get => _downloadStatusText;
            set { _downloadStatusText = value; OnPropertyChanged(); }
        }

        public List<SerieListItemModel> FilteredSeries
        {
            get { return _filteredSeries; }
            set { _filteredSeries = value; OnPropertyChanged(); }
        }

        public SerieDetailsModel SerieDetails
        {
            get
            {
                return _serieDetails;
            }
            set
            {
                _serieDetails = value;
                if (value != null)
                {
                    foreach (ChapterListItemModel chapter in _serieDetails.Taggings)
                    {
                        chapter.IsSelected = ChaptersToDownload.Where(x => x.Permalink.Equals(chapter.Permalink)).Count() > 0;
                    }
                    OnPropertyChanged();
                }

            }
        }

        public ObservableCollection<ChapterListItemModel> ChaptersToDownload
        {
            get { return _chaptersToDownload; }
            set
            {
                _chaptersToDownload = value;
                OnPropertyChanged();
            }
        }

        public SerieListItemModel SelectedSerie
        {
            get { return _selectedSerie; }
            set
            {
                _selectedSerie = value;
                OnPropertyChanged();
                _ = LoadSerieDetails();
            }
        }

        #endregion

        #region Async Init
        private async Task LoadSerieDetails()
        {
            SerieDetails = await _service.GetSerieDetails(_selectedSerie);
        }
        #endregion

        // Commands

        #region ChapterClick (CheckBox)
        public ICommand ChapterClick
        {
            get => _chapterClick;
            set { _chapterClick = value; OnPropertyChanged(); }
        }

        private void OnChapterClick(CheckBox c)
        {
            bool isChecked = (bool)c.IsChecked;
            ChapterListItemModel chapterItem = (ChapterListItemModel)c.DataContext;
            chapterItem.IsSelected = isChecked;

            if (isChecked)
                ChaptersToDownload.Add(chapterItem);
            else
                ChaptersToDownload.Remove(chapterItem);
        }
        #endregion

        #region SelectAllChapterChange (CheckBox)
        public ICommand SelectAllChaptersChange
        {
            get => _selectAllChaptersChange;
            set { _selectAllChaptersChange = value; OnPropertyChanged(); }
        }

        private void OnSelectAllChaptersChange(CheckBox c)
        {
            bool isChecked = (bool)c.IsChecked;

            foreach (ChapterListItemModel chapterItem in _serieDetails.Taggings)
            {
                if (!chapterItem.Exists)
                {
                    chapterItem.IsChecked = isChecked;
                    if (isChecked && !ChaptersToDownload.Contains(chapterItem))
                        ChaptersToDownload.Add(chapterItem);
                    else if (!isChecked && ChaptersToDownload.Contains(chapterItem))
                        ChaptersToDownload.Remove(chapterItem);
                }
            }

        }

        private bool CanExecuteSelectAllChaptersChange(object param)
        {
            return _serieDetails != null && !_isDownloading;
        }
        #endregion

        #region DownloadClick (Button)

        public ICommand DownloadClick
        {
            get { return _downloadClick; }
            set { _downloadClick = value; OnPropertyChanged(); }
        }

        public async Task OnDownloadClick()
        {
            await Task.Run(() =>
            {
                IsDownloading = true;
                _service.AssignChapterModels(ChaptersToDownload.ToList());
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

        private bool CanExecuteDownloadClick(object param)
        {
            return !_isDownloading;
        }

        public bool IsDownloading
        {
            get { return _isDownloading; }
            set
            {
                _isDownloading = value;
                OnPropertyChanged();
            }
        }

        private void InitProgressBarDatas()
        {

            int pageCount = 0;
            foreach (var item in ChaptersToDownload)
            {
                pageCount += item.Chapter.Pages.Count;
            }

            PagesToDownloadCount = pageCount;
            PagesDownloadedCount = 0;
        }

        private void ResetProgressBarDatas()
        {
            PagesToDownloadCount = 1;
            PagesDownloadedCount = 0;
        }

        public int PagesToDownloadCount
        {
            get { return _pagesToDownloadCount; }
            set { _pagesToDownloadCount = value; OnPropertyChanged(); }
        }

        public int PagesDownloadedCount
        {
            get { return _pagesDownloadedCount; }
            set
            {
                _pagesDownloadedCount = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region MangaFilterChanged (TextBox On KeyUp)
        public ICommand MangaFilterChanged
        {
            get => _mangaFilterChanged;
            set { _mangaFilterChanged = value; OnPropertyChanged(); }
        }

        private void OnMangaFilterChanged(string filterString)
        {
            IEnumerable<SerieListItemModel> t = from serie in Series
                                                where serie.Name.ToLower().IndexOf(filterString.ToLower()) >= 0
                                                select serie;
            FilteredSeries = t.ToList();
        }
        #endregion

        // Events

        #region Events 
        private void OnPageDownloadStateChanged(object param, string s)
        {
            DownloadStatusText = s;
        }

        #endregion

    }
}
