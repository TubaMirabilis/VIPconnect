using Microsoft.AspNetCore.Identity;
using System;

namespace ProjectX.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? DateOfBirth { get; set; }
        public string SightCategory { get; set; }
        public string EmploymentStatus { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string Industry { get; set; }
        public DateTime WorkingSince { get; set; }
        public int BreakTime { get; set; }
        public string WorkRoles { get; set; }
        public string Strengths { get; set; }
        public string IsParent { get; set; }
        public string Biog { get; set; }
        public DateTime Joined { get; set; }
    }
}