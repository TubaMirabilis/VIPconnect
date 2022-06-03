using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectX.Models
{
    public class Category : IEntity
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string ImageSrc { get; set; }
        public DateTime DateCreated { get; set; }
    }
}