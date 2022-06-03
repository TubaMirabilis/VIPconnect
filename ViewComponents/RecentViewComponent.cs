using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.ViewComponents
{
    public class RecentViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public RecentViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _unitOfWork.DiscussionRepository.Get(includeProperties: "Replies,Likes", orderBy: x => x.OrderByDescending(x => x.DateCreated), pageNumber: 1, pageSize: 10));
        }
    }
}