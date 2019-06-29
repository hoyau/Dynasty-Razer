using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.Models
{
    public class ChapterModel
    {
        public string Title { get; set; }
        public string Permalink { get; set; }
        public List<ChapterPage> Pages { get; set; }
        public List<ChapterTag> Tags { get; set; }
    }
}

//Example: https://dynasty-scans.com/chapters/an_absurd_relationship_ch01.json
