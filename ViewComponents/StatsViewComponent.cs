using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.ViewComponents
{
    public class StatsViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public StatsViewComponent(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.Users = await _userManager.Users.CountAsync();
            ViewBag.Discussions = await _unitOfWork.DiscussionRepository.Count();
            if(!string.IsNullOrEmpty(_userManager.Users.OrderBy(x => x.Joined)?.LastOrDefault()?.UserName))
            {
                ViewBag.Newest = _userManager.Users.OrderBy(x => x.Joined).LastOrDefault().UserName;
            }
            else
            {
                ViewBag.Newest = "";
            }
            return View();
        }
    }
}