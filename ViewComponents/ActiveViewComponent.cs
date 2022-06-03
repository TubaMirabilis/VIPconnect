using Microsoft.AspNetCore.Mvc;
using ProjectX.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.ViewComponents
{
    public class ActiveViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ActiveViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if(!User.IsInRole("Administrator"))
            {
                var name = User.Identity.Name;
                return View(await _unitOfWork.SupportTicketRepository.Get(filter: x => x.Creator == name && x.IsResolved == false, orderBy: x => x.OrderByDescending(x => x.DateCreated), pageNumber: 1, pageSize: int.MaxValue));
            }
            return View(await _unitOfWork.SupportTicketRepository.Get(filter: x => x.IsResolved == false, orderBy: x => x.OrderByDescending(x => x.DateCreated), pageNumber: 1, pageSize: int.MaxValue));
        }
    }
}