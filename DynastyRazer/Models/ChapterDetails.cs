using System.Collections.Generic;

namespace DynastyRazer.Models
{
    public class ChapterDetails
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public List<ChapterPage> Pages { get; set; }
    }
}

