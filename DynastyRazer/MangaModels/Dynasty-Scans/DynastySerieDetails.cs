using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.MangaModels.Dynasty_Scans
{
    public class DynastySerieDetails
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<DynastyChapter> Taggings { get; set; }
    }
}
