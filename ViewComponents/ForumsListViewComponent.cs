using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.ViewComponents
{
    public class ForumsListViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ForumsListViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _unitOfWork.CategoryRepository.Get(orderBy: x => x.OrderBy(x => x.DateCreated), pageSize: int.MaxValue));
        }
    }
}