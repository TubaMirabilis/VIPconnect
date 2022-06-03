using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.ViewComponents
{
    public class ForumsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ForumsViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _unitOfWork.CategoryRepository.Get(pageSize: int.MaxValue));
        }
    }
}