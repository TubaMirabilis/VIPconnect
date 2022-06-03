using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectX.Models;
using System.Collections.Generic;

namespace ProjectX.ViewModels
{
    public class CreateDiscussionViewModel
    {
        public Discussion Discussion { get; set; }
        public List<SelectListItem> Categories { get; set; }
    }
}