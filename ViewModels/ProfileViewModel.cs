using System;
using System.Collections.Generic;
using System.Linq;
using ProjectX.Models;

namespace ProjectX.ViewModels
{
    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<TextContent> Contributions { get; set; }
        public IEnumerable<Discussion> Discussions { get; set; }
        public string Image { get; set; }
        public int PageNumber { get; set; }
        public IEnumerable<int> PageNumbers { get; set; }
        public int PageCount { get; set; }
        public bool HasMore { get; set; }
        public string GetDiscussionTitle(Guid? id)
            => Discussions.FirstOrDefault(d => d.Id == id).Title;
        
        public string GetDiscussionCreator(Guid? id)
            => Discussions.FirstOrDefault(d => d.Id == id).Creator;
    }
}