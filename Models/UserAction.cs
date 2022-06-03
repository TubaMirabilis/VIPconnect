using System;

namespace ProjectX.Models
{
    public class UserAction : IEntity
    {
        public Guid Id { get; set; }
        public string Details { get; set; }
        public Guid? DiscussionId { get; set; }
        public Guid? ReplyId { get; set; }
        public Guid? TicketId { get; set; }
        public string UserId { get; set; }
        public DateTime LoggedAt { get; set; }
    }
}