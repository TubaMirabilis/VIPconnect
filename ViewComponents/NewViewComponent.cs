using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectX.Data;
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
            var categories = await _unitOfWork.CategoryRepository.Get(pageSize: int.MaxValue);
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