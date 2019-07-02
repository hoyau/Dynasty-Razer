using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.MangaModels.Dynasty_Scans
{
    public class DynastyChapter
    {
        public string Title { get; set; }
        public string Permalink { get; set; }
        public List<DynastyPage> Pages { get; set; }
        public List<DynastyTag> Tags { get; set; }
       
    }
}
