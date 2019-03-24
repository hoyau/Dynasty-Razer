using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DynastyRazer.Models;
using Newtonsoft.Json;

namespace DynastyRazer.Services
{
    public class DynastyScansService : IMangaProviderService
    {

        public MangaProviderConfiguration Config { get; set; }
        public List<ChapterListItemModel> DownloadList { get; set; }
        public event EventHandler<string> PageDownloadStateChanged;

        public DynastyScansService(MangaProviderConfiguration config)
        {
            Config = config;
            DownloadList = new List<ChapterListItemModel>();

        }

 

        // Implementation

        public void AssignChapterModels(List<ChapterListItemModel> list)
        {
            HttpClient client = null;
            foreach (ChapterListItemModel item in list)
            {
                try
                {
                    client = new HttpClient();
                    string url = $"https://dynasty-scans.com/chapters/{item.Permalink}.json";
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    string json = response.Content.ReadAsStringAsync().Result;

                    ChapterModel chapter = JsonConvert.DeserializeObject<ChapterModel>(json);
                    item.Chapter = chapter;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        public void DownloadPage(ChapterPageModel page, string mangaName = null, string chapterName = null)
        {
            // Create Directory if it doesn't exist

            Directory.CreateDirectory($@"{Config.SavePath}\{mangaName}\{chapterName}");

            PageDownloadStateChanged.Invoke(this, $"Downloading {chapterName} Page {page.Name}");
            using (WebClient client = new WebClient())
            {
                Uri uri = new Uri("https://dynasty-scans.com" + page.Url);
                string fileName = uri.Segments[uri.Segments.Length - 1];
                client.DownloadFile(uri, $@"{Config.SavePath}\{mangaName}\{chapterName}\{fileName}");
            }
            PageDownloadStateChanged.Invoke(this, $"Success");
        }

        public List<SerieListItemModel> GetAllSeries()
        {
            List<SerieListItemModel> series = null;

            using (var client = new HttpClient())
            {
                string url = "https://dynasty-scans.com/series.json";
                HttpResponseMessage response = client.GetAsync(url).Result;
                string json = response.Content.ReadAsStringAsync().Result;

                // Remove {"#": and } 
                json = json.Substring(5, json.Length - 6);
                // Replace ],"[A-Z]":[ with , that every object is in 1 Array and not categorized
                json = Regex.Replace(json, @"\],""[A-Z]"":\[", ",");

                series = JsonConvert.DeserializeObject<List<SerieListItemModel>>(json);

                foreach (var serie in series)
                    serie.Name = string.Join("", serie.Name.Split(Path.GetInvalidFileNameChars()));
            }

            return series;
        }

        public SerieDetailsModel GetSerieDetails(SerieListItemModel filter)
        {
            if (filter == null)
                return null;

            SerieDetailsModel serie = null;

            using (var client = new HttpClient())
            {
                string url = $"https://dynasty-scans.com/series/{filter.Permalink}.json";
                HttpResponseMessage response = client.GetAsync(url).Result;
                string json = response.Content.ReadAsStringAsync().Result;

                serie = JsonConvert.DeserializeObject<SerieDetailsModel>(json);

                serie.Taggings = serie.Taggings.Where(x => !String.IsNullOrEmpty(x.Title)).ToList();

                foreach (var tag in serie.Taggings)
                    tag.Title = string.Join("", tag.Title.Split(Path.GetInvalidFileNameChars()));

            }

            SetChapterIsAlreadyDownloadedState(serie);

            return serie;
        }

        // Helper

        private void SetChapterIsAlreadyDownloadedState(SerieDetailsModel serie)
        {
            if (serie == null) return;

            foreach (ChapterListItemModel chapter in serie.Taggings)
            {
                string fullPath = $@"{Config.SavePath}\{serie.Name}\{chapter.Title}";
                chapter.Exists = Directory.Exists(fullPath);
            }
        }


    }
}

/*
 * https://dynasty-scans.com/series.json
 * "#": [] mit {name und permalink}
 * 
 */
