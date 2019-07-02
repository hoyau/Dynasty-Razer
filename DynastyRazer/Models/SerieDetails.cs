using DynastyRazer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.Models
{
    public class SerieDetails
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public List<ChapterListItem> Chapters { get; set; }
    }


}
