using System.Collections.Generic;
using ProjectX.Models;

namespace ProjectX.ViewModels
{
    public class IndexViewModel
    {
        public int PageNumber { get; set; }
        public string Category { get; set; }
        public IEnumerable<int> PageNumbers { get; set; }
        public int PageCount { get; set; }
        public bool HasMore { get; set; }
        public IEnumerable<Discussion> Discussions { get; set; }
    }
}