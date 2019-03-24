using DynastyRazer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DynastyRazer.Services
{
    public interface IMangaProviderService
    {
        MangaProviderConfiguration Config { get; set; }

        List<SerieListItemModel> GetAllSeries();
        SerieDetailsModel GetSerieDetails(SerieListItemModel filter);
        void AssignChapterModels(List<ChapterListItemModel> list);
        void DownloadPage(ChapterPageModel page, string mangaName, string chapterName);
        event EventHandler<string> PageDownloadStateChanged;
      
    }
}


