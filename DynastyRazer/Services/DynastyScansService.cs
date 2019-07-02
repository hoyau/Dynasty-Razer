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
using DynastyRazer.MangaMapper.Dynasty_Scans;
using DynastyRazer.MangaModels.Dynasty_Scans;
using DynastyRazer.Models;
using DynastyRazer.ViewModels;
using Newtonsoft.Json;

namespace DynastyRazer.Services
{
    public class DynastyScansService : IMangaProviderService
    {
        public event EventHandler<string> PageDownloadStateChanged;

        public DynastyScansService()
        {

        }

        public async Task AssignChapter(List<ChapterListItem> list)
        {
            foreach (ChapterListItem item in list)
            {
                using (var client = new HttpClient())
                {
                    string url = $"https://dynasty-scans.com/chapters/{item.Url}.json";
                    HttpResponseMessage response = await client.GetAsync(url);
                    string json = await response.Content.ReadAsStringAsync();

                    DynastyChapter internChapter = JsonConvert.DeserializeObject<DynastyChapter>(json);
                    ChapterDetails chapterDetails = DynastyMapper.MapChapterDetails(internChapter);
                    item.ChapterDetails = chapterDetails;
                }
            }
        }

        public async Task<List<SerieListItem>> RetrieveAllSeries()
        {
            List<DynastySerieListItem> internSeries = null;

            using (var client = new HttpClient())
            {
                string url = "https://dynasty-scans.com/series.json";
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                // Remove {"#": and } 
                json = json.Substring(5, json.Length - 6);
                // Replace ],"[A-Z]":[ with , that every object is in 1 Array and not categorized
                json = Regex.Replace(json, @"\],""[A-Z]"":\[", ",");

                internSeries = JsonConvert.DeserializeObject<List<DynastySerieListItem>>(json);

                foreach (var serie in internSeries)
                    serie.Name = string.Join("", serie.Name.Split(Path.GetInvalidFileNameChars()));
            }

            List<SerieListItem> series = DynastyMapper.MapSerieListItems(internSeries);

            return series;
        }

        public async Task<SerieDetails> RetrieveSerieDetails(SerieListItem filter)
        {
            if (filter == null)
                return null;

            DynastySerieDetails internSerie = null;

            using (var client = new HttpClient())
            {
                string url = $"https://dynasty-scans.com/series/{filter.Url}.json";
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                internSerie = JsonConvert.DeserializeObject<DynastySerieDetails>(json);
                internSerie.Taggings = internSerie.Taggings.Where(x => !String.IsNullOrEmpty(x.Title)).ToList();

                foreach (var tag in internSerie.Taggings)
                    tag.Title = string.Join("", tag.Title.Split(Path.GetInvalidFileNameChars()));
            }

            SerieDetails serie = DynastyMapper.MapSerieDetails(internSerie, filter.Title, filter.Url);
            StorageService.SetChapterIsAlreadyDownloadedState(serie);
            return serie;
        }

        public async Task StartDownload(List<ChapterListItem> chaptersToDownload)
        {
            foreach (var item in chaptersToDownload)
            {
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
                Uri uri = new Uri("https://dynasty-scans.com" + page.Url);
                string fileName = uri.Segments[uri.Segments.Length - 1];
                client.DownloadFile(uri, $@"{StorageService.SavePath}\{mangaName}\{chapterName}\{fileName}");
            }

            PageDownloadStateChanged.Invoke(this, $"Success");
            return Task.CompletedTask;
        }
    }

}

/*
 * https://dynasty-scans.com/series.json
 * "#": [] mit {name und permalink}
 * 
 */
