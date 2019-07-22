using DynastyRazer.MangaModels.Dynasty_Scans;
using DynastyRazer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.MangaMapper.Dynasty_Scans
{
    public class DynastyMapper
    {
        public static SerieDetails MapSerieDetails(DynastySerieDetails internSerieDetails, string mangaTitle, string mangaUrl)
        {
            if (internSerieDetails == null)
                return null;

            List<ChapterListItem> chapters = internSerieDetails.Taggings?.Select(c => new ChapterListItem
            {
                Title = c.Title,
                Url = c.Permalink,
                MangaTitle = mangaTitle,
            })?.ToList();

            SerieDetails serie = new SerieDetails
            {
                Name = internSerieDetails.Name,
                Chapters = chapters,
                Url = mangaUrl,
                Image = $"http://dynasty-scans.com{internSerieDetails.Cover}",
            };

            return serie;

        }

        public static ChapterDetails MapChapterDetails(DynastyChapter internChapter)
        {
            if (internChapter == null)
                return null;

            List<ChapterPage> pages = internChapter.Pages?.Select(p => new ChapterPage
            {
                Name = p.Name,
                Url = p.Url,
            })?.ToList();

            ChapterDetails chapterDetails = new ChapterDetails
            {
                Title = internChapter.Title,
                Pages = pages,
                Url = internChapter.Permalink,
            };
            return chapterDetails;
        }

        public static List<SerieListItem> MapSerieListItems(List<DynastySerieListItem> internSeries)
        {
            if (internSeries == null)
                return null;

            List<SerieListItem> series = internSeries.Select(s => new SerieListItem
            {
                Title = s.Name,
                Url = s.Permalink
            })?.ToList();

            return series;
        }
    }
}
