using DynastyRazer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.Services
{
    public class StorageService
    {
        public static string SavePath
        {
            get => Properties.Settings.Default.SavePath;
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
