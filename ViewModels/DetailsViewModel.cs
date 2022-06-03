using System.Collections.Generic;
using ProjectX.Models;

namespace ProjectX.ViewModels
{
    public class DetailsViewModel
    {
        public int PageNumber { get; set; }
        public IEnumerable<int> PageNumbers { get; set; }
        public int PageCount { get; set; }
        public bool HasMore { get; set; }
        public Discussion Discussion { get; set; }
        public IEnumerable<Reply> Replies { get; set; }
    }
}