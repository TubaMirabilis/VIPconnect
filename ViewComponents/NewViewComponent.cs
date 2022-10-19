using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectX.Data;
using ProjectX.Models;
using ProjectX.ViewModels;

namespace ProjectX.ViewComponents
{
    public class NewViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public NewViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = Enumerable.Empty<Category>();
            if(User.IsInRole("Moderator") || User.IsInRole("Administrator"))
            {
                categories = await _unitOfWork.CategoryRepository.Get(orderBy: x => x.OrderBy(x => x.DateCreated), pageSize: int.MaxValue);
            }
            else
            {
                categories = await _unitOfWork.CategoryRepository.Get(orderBy: x => x.OrderBy(x => x.DateCreated), filter: x => x.StaffOnly == false, pageSize: int.MaxValue);
            }
            var viewmodel = new CreateDiscussionViewModel
            {
                Categories = new List<SelectListItem>()
            };
            foreach (var c in categories)
            {
                viewmodel.Categories.Add(new SelectListItem
                {
                    Value = c.Name, Text = c.Name
                });
            }
            return View(viewmodel);
        }
    }
}