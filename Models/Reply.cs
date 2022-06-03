using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectX.Models
{
    public class Reply : TextContent
    {
        public override Guid Id { get; set; }
        public string Respondent { get; set; }
        public override string Content { get; set; }
        public override DateTime DateCreated { get; set; }
        [NotMapped]
        public override string Image { get; set; }
        public Guid? DiscussionId { get; set; }
        public Discussion Discussion { get; set; }
        public Guid? SupportTicketId { get; set; }
        public SupportTicket SupportTicket { get; set; }
    }
}