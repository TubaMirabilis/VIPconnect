using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectX.Models
{
    public class Discussion : TextContent
    {
        public override Guid Id { get; set; }
        public string Creator { get; set; }
        [Required]
        public string Title { get; set; }
        public override DateTime DateCreated { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public override string Content { get; set; }
        public override string Image { get; set; }
        public string CreatorId { get; set; }
        public ICollection<DiscussionLike> Likes { get; set; }
        public ICollection<Reply> Replies { get; set; }
        public Discussion()
        {
            this.Replies = new List<Reply>();
            this.Likes = new List<DiscussionLike>();
        }
    }
}
