using DynastyRazer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.Models
{
    //https://dynasty-scans.com/series/an_absurd_relationship.json
    public class SerieDetails
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<ChapterListItem> Taggings { get; set; }
    }


}
