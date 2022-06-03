using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectX.Models
{
    public class SupportTicket : TextContent
    {
        public override Guid Id { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public override string Content { get; set; }
        public override DateTime DateCreated { get; set; }
        public string Creator { get; set; }
        [NotMapped]
        public override string Image { get; set; }
        public bool IsResolved { get; set; }
        public ICollection<Reply> Replies { get; set; }
        public SupportTicket()
        {
            this.Replies = new List<Reply>();
        }
    }
}