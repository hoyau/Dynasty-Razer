using DynastyRazer.MangaModels.DynastyScans;
using DynastyRazer.Models;
using System.Collections.Generic;
using System.Linq;

namespace DynastyRazer.MangaMapper
{
    public static class DynastyMapper
    {
        public static SerieDetails MapSerieDetails(DynastySerieDetails externSerieDetails, string mangaTitle, string mangaUrl)
        {
            if (externSerieDetails == null)
                return null;

            var chapters = externSerieDetails.Taggings?.Select(c => new ChapterListItem
            {
                Title = c.Title,
                Url = c.Permalink,
                MangaTitle = mangaTitle,
            }).ToList();

            return new SerieDetails
            {
                Name = externSerieDetails.Name,
                Chapters = chapters,
                Url = mangaUrl,
                ImageUrl = $"http://dynasty-scans.com{externSerieDetails.Cover}",
            };
        }

        public static ChapterDetails MapChapterDetails(DynastyChapter externChapter)
        {
            if (externChapter == null)
                return null;

            var pages = externChapter.Pages?.Select(p => new ChapterPage
            {
                Name = p.Name,
                Url = p.Url,
            }).ToList();

            return new ChapterDetails
            {
                Title = externChapter.Title,
                Pages = pages,
                Url = externChapter.Permalink,
            };
        }

        public static List<SerieListItem> MapSerieListItems(List<DynastySerieListItem> externSeries)
        {
            if (externSeries == null)
                return null;

            return externSeries.Select(s => new SerieListItem
            {
                Title = s.Name,
                Url = s.Permalink
            }).ToList();
        }
    }
}
