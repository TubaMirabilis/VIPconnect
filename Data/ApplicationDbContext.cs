using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectX.Models;

namespace ProjectX.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ProjectX.Models.Discussion> Discussion { get; set; }
        public DbSet<ProjectX.Models.Reply> Reply { get; set; }
        public DbSet<ProjectX.Models.DiscussionLike> DiscussionLike { get; set; }
        public DbSet<ProjectX.Models.UserAction> UserAction { get; set; }
        public DbSet<ProjectX.Models.SupportTicket> SupportTicket { get; set; }
        public DbSet<ProjectX.Models.Category> Category { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reply>().Property(e => e.Id).ValueGeneratedNever();
            modelBuilder.Entity<DiscussionLike>().Property(e => e.Id).ValueGeneratedNever();
            modelBuilder.Entity<Category>().HasData( new Category
            {
                Id = Guid.NewGuid(),
                Name = "Introductions",
                Description = "Newcomers should divulge their innermost secrets in this forum.",
                ImageSrc = "introductions.svg",
                DateCreated = DateTime.UtcNow
            }, new Category
            {
                Id = Guid.NewGuid(),
                Name = "The Office",
                Description = "For professionals to discuss professional stuff.",
                ImageSrc = "office.svg",
                DateCreated = DateTime.UtcNow
            }, new Category
            {
                Id = Guid.NewGuid(),
                Name = "Job-hunting",
                Description = "For job application advice.",
                ImageSrc = "hunting.svg",
                DateCreated = DateTime.UtcNow
            }, new Category
            {
                Id = Guid.NewGuid(),
                Name = "Access to Work",
                Description = "A forum for discussing the UK Government scheme of the same name.",
                ImageSrc = "universal-access-circle.svg",
                DateCreated = DateTime.UtcNow
            }, new Category
            {
                Id = Guid.NewGuid(),
                Name = "Parents' Zone",
                Description = "For parents to discuss maternal/paternal stuff.",
                ImageSrc = "parents-child.svg",
                DateCreated = DateTime.UtcNow
            }, new Category
            {
                Id = Guid.NewGuid(),
                Name = "College Library",
                Description = "For students to discuss matters pertaining to academic life.",
                ImageSrc = "students.svg",
                DateCreated = DateTime.UtcNow
            });
        }
    }
}