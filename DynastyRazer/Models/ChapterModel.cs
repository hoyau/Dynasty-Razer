using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.Models
{
    //https://dynasty-scans.com/chapters/an_absurd_relationship_ch01.json
    public class ChapterModel
    {
        public string Title { get; set; }
        public string Permalink { get; set; }
        public List<ChapterPageModel> Pages { get; set; }
        public List<ChapterTagModel> Tags { get; set; }
    }

    public class ChapterPageModel
    {
        //    06
        public string Name { get; set; }
        //    /system/releases/000/011/482/Irrational_Us_06.jpg
        public string Url { get; set; }
    }

    public class ChapterTagModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Permalink { get; set; }
    }
}
