using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using DynastyRazer.Models;
using DynastyRazer.ViewModels;
using Newtonsoft.Json;

namespace DynastyRazer.Services
{
    public class DynastyScansService : IMangaProviderService
    {
        public MangaProviderConfiguration Configuration { get; set; }
        public List<ChapterListItem> DownloadList { get; set; }

        public event EventHandler<string> PageDownloadStateChanged;

        public DynastyScansService(MangaProviderConfiguration configuration)
        {
            Configuration = configuration;
            DownloadList = new List<ChapterListItem>();
        }

        public async Task AssignChapter(List<ChapterListItem> list)
        {
            foreach (ChapterListItem item in list)
            {
                using (var client = new HttpClient())
                {
                    string url = $"https://dynasty-scans.com/chapters/{item.Permalink}.json";
                    HttpResponseMessage response = await client.GetAsync(url);
                    string json = await response.Content.ReadAsStringAsync();

                    Chapter chapter = JsonConvert.DeserializeObject<Chapter>(json);
                    item.Chapter = chapter;
                }
            }
        }

        public Task DownloadPage(ChapterPage page, string mangaName = null, string chapterName = null)
        {
            Directory.CreateDirectory($@"{Configuration.SavePath}\{mangaName}\{chapterName}");
            PageDownloadStateChanged.Invoke(this, $"Downloading {chapterName} Page {page.Name}");

            using (var client = new WebClient())
            {
                Uri uri = new Uri("https://dynasty-scans.com" + page.Url);
                string fileName = uri.Segments[uri.Segments.Length - 1];
                client.DownloadFile(uri, $@"{Configuration.SavePath}\{mangaName}\{chapterName}\{fileName}");
            }

            PageDownloadStateChanged.Invoke(this, $"Success");
            return Task.CompletedTask;
        }

        public async Task<List<SerieListItem>> RetrieveAllSeries()
        {
            List<SerieListItem> series = null;

            using (var client = new HttpClient())
            {
                string url = "https://dynasty-scans.com/series.json";
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                // Remove {"#": and } 
                json = json.Substring(5, json.Length - 6);
                // Replace ],"[A-Z]":[ with , that every object is in 1 Array and not categorized
                json = Regex.Replace(json, @"\],""[A-Z]"":\[", ",");

                series = JsonConvert.DeserializeObject<List<SerieListItem>>(json);

                foreach (var serie in series)
                    serie.Name = string.Join("", serie.Name.Split(Path.GetInvalidFileNameChars()));
            }

            return series;
        }

        public async Task<SerieDetails> RetrieveSerieDetails(SerieListItem filter)
        {
            if (filter == null)
                return null;

            SerieDetails serie = null;

            using (var client = new HttpClient())
            {
                string url = $"https://dynasty-scans.com/series/{filter.Permalink}.json";
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                serie = JsonConvert.DeserializeObject<SerieDetails>(json);

                serie.Taggings = serie.Taggings.Where(x => !String.IsNullOrEmpty(x.Title)).ToList();

                foreach (var tag in serie.Taggings)
                    tag.Title = string.Join("", tag.Title.Split(Path.GetInvalidFileNameChars()));

            }

            SetChapterIsAlreadyDownloadedState(serie);
            return serie;
        }

        private void SetChapterIsAlreadyDownloadedState(SerieDetails serie)
        {
            if (serie == null)
                return;

            foreach (ChapterListItem chapter in serie.Taggings)
            {
                string fullPath = $@"{Configuration.SavePath}\{serie.Name}\{chapter.Title}";
                chapter.IsLocallySaved = Directory.Exists(fullPath);
            }
        }


    }
}

/*
 * https://dynasty-scans.com/series.json
 * "#": [] mit {name und permalink}
 * 
 */
