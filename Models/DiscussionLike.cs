using System;

namespace ProjectX.Models
{
    public class DiscussionLike : IEntity
    {
        public Guid Id { get; set; }
        public string IpAddress { get; set; }
        public Guid DiscussionId { get; set; }
        public Discussion Discussion { get; set; }
    }
}