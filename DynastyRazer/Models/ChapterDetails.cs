using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynastyRazer.Models
{
    public class ChapterDetails
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public List<ChapterPage> Pages { get; set; }
    }
}

