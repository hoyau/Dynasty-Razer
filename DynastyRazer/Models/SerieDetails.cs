using System.Collections.Generic;

namespace DynastyRazer.Models
{
    public class SerieDetails
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public List<ChapterListItem> Chapters { get; set; }
    }
}
