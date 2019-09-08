using DynastyRazer.Models;
using System.IO;

namespace DynastyRazer.Services
{
    public class StorageService
    {
        public static string SavePath
        {
            get => string.IsNullOrEmpty(Properties.Settings.Default.SavePath)
                ? Directory.GetCurrentDirectory() 
                : Properties.Settings.Default.SavePath;
            set
            {
                Properties.Settings.Default.SavePath = value;
                Properties.Settings.Default.Save();
            }
        }

        public static void SetChapterIsAlreadyDownloadedState(SerieDetails serie)
        {
            if (serie == null)
                return;

            foreach (ChapterListItem chapter in serie.Chapters)
            {
                string fullPath = $@"{SavePath}\{serie.Name}\{chapter.Title}";
                chapter.IsLocallySaved = Directory.Exists(fullPath);
            }
        }
    }
}
