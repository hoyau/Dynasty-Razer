using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.Models
{
    public class MangaProviderConfiguration
    {
        public string SavePath {
            get => Properties.Settings.Default.SavePath;
            set {
                Properties.Settings.Default.SavePath = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
