using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryUWP
{
    public class Book
    {
        public List<Author> Authors { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Genre { get; set; }
        public int Published { get; set; }
    }
}
