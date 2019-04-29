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

        Task<List<SerieListItemModel>> GetAllSeries();
        Task<SerieDetailsModel> GetSerieDetails(SerieListItemModel filter);
        Task AssignChapterModels(List<ChapterListItemModel> list);
        Task DownloadPage(ChapterPageModel page, string mangaName, string chapterName);
        event EventHandler<string> PageDownloadStateChanged;
      
    }
}


