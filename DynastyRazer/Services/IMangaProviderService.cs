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
        /// <summary>
        /// Should be invoked on MangaProvider change.
        /// </summary>
        /// <returns></returns>
        Task<List<SerieListItem>> RetrieveAllSeries();

        /// <summary>
        /// Retrieves Serie Details. Should be lazy loaded (after serie change)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<SerieDetails> RetrieveSerieDetails(SerieListItem filter);

        /// <summary>
        /// Assign ChapterDetails to the ChapterListItem. Should be lazy loaded (on download start)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task AssignChapter(List<ChapterListItem> list);

        /// <summary>
        /// Start Download
        /// </summary>
        /// <param name="page"></param>
        /// <param name="mangaName"></param>
        /// <param name="chapterName"></param>
        /// <returns></returns>
        Task StartDownload(List<ChapterListItem> chaptersToDownload);

        event EventHandler<string> PageDownloadStateChanged;
      
    }
}


