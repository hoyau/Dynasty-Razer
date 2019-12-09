using DynastyRazer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DynastyRazer.Services
{
    public interface IMangaProviderService
    {
        Task<List<SerieListItem>> RetrieveAllSeries();

        Task<SerieDetails> RetrieveSerieDetails(SerieListItem filter);

        /// <summary>
        /// Assign ChapterDetails to the ChapterListItem. Should be lazy loaded (on download start)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task AssignChapterDetails(List<ChapterListItem> list);

        Task StartDownload(List<ChapterListItem> chaptersToDownload);

        event EventHandler<string> PageDownloadStateChanged;

    }
}


