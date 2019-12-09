using DynastyRazer.MangaMapper;
using DynastyRazer.MangaModels.DynastyScans;
using DynastyRazer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynastyRazer.Services
{
    public class DynastyScansService : IMangaProviderService
    {
        public event EventHandler<string> PageDownloadStateChanged;

        public async Task AssignChapterDetails(List<ChapterListItem> list)
        {
            if (list == null || !list.Any())
                return;

            using var client = new HttpClient();

            foreach (ChapterListItem item in list)
            {
                var url = $"https://dynasty-scans.com/chapters/{item.Url}.json";
                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                var externChapter = JsonConvert.DeserializeObject<DynastyChapter>(json);
                var chapterDetails = DynastyMapper.MapChapterDetails(externChapter);
                item.ChapterDetails = chapterDetails;
            }
        }

        public async Task<List<SerieListItem>> RetrieveAllSeries()
        {
            List<DynastySerieListItem> externSeries = null;

            using var client = new HttpClient();

            var url = "https://dynasty-scans.com/series.json";
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            // Remove {"#": and } 
            json = json.Substring(5, json.Length - 6);
            // Replace ],"[A-Z]":[ with , that every object is in 1 Array and not categorized
            json = Regex.Replace(json, @"\],""[A-Z]"":\[", ",");

            externSeries = JsonConvert.DeserializeObject<List<DynastySerieListItem>>(json);

            foreach (var serie in externSeries)
            {
                serie.Name = string.Join("", serie.Name.Split(Path.GetInvalidFileNameChars()));
            }

            return DynastyMapper.MapSerieListItems(externSeries);
        }

        public async Task<SerieDetails> RetrieveSerieDetails(SerieListItem filter)
        {
            if (filter == null)
                return null;

            DynastySerieDetails externSerie = null;

            using var client = new HttpClient();

            var url = $"https://dynasty-scans.com/series/{filter.Url}.json";
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            externSerie = JsonConvert.DeserializeObject<DynastySerieDetails>(json);
            externSerie.Taggings = externSerie.Taggings.Where(x => !string.IsNullOrEmpty(x.Title)).ToList();

            foreach (var tag in externSerie.Taggings)
            {
                tag.Title = string.Join("", tag.Title.Split(Path.GetInvalidFileNameChars()));
            }

            var serie = DynastyMapper.MapSerieDetails(externSerie, filter.Title, filter.Url);
            StorageService.SetChapterIsAlreadyDownloadedState(serie);
            return serie;
        }

        public async Task StartDownload(List<ChapterListItem> chaptersToDownload)
        {
            if (chaptersToDownload == null)
                return;

            foreach (var item in chaptersToDownload)
            {
                if (item.ChapterDetails?.Pages == null)
                    continue;

                foreach (var page in item.ChapterDetails.Pages)
                {
                    await DownloadPage(page, item.MangaTitle, item.Title);
                }
            }
        }

        private Task DownloadPage(ChapterPage page, string mangaName = null, string chapterName = null)
        {
            Directory.CreateDirectory($@"{StorageService.SavePath}\{mangaName}\{chapterName}");
            PageDownloadStateChanged.Invoke(this, $"Downloading {chapterName} Page {page.Name}");

            using (var client = new WebClient())
            {
                var uri = new Uri("https://dynasty-scans.com" + page.Url);
                var fileName = uri.Segments[uri.Segments.Length - 1];
                client.DownloadFile(uri, $@"{StorageService.SavePath}\{mangaName}\{chapterName}\{fileName}");
            }

            return Task.CompletedTask;
        }
    }
}