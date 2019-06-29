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
        MangaProviderConfiguration Configuration { get; set; }

        Task<List<SerieListItem>> RetrieveAllSeries();
        Task<SerieDetails> RetrieveSerieDetails(SerieListItem filter);
        Task AssignChapter(List<ChapterListItem> list);
        Task DownloadPage(ChapterPage page, string mangaName, string chapterName);
        event EventHandler<string> PageDownloadStateChanged;
      
    }
}


