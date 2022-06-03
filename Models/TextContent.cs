using System;

namespace ProjectX.Models
{
    public abstract class TextContent : IEntity
    {
        public abstract Guid Id { get; set; }
        public abstract string Content { get; set; }
        public abstract DateTime DateCreated { get; set; }
        public abstract string Image { get; set; }
    }
}