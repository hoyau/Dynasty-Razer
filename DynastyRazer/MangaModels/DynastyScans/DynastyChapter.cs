using DynastyRazer.MangaModels.DynastyScans;
using System.Collections.Generic;

namespace DynastyRazer.MangaModels.DynastyScans
{
    public class DynastyChapter
    {
        public string Title { get; set; }
        public string Permalink { get; set; }
        public List<DynastyPage> Pages { get; set; }
        public List<DynastyTag> Tags { get; set; }
    }
}
