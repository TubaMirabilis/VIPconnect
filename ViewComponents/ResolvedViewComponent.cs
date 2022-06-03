using Microsoft.AspNetCore.Mvc;
using ProjectX.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.ViewComponents
{
    public class ResolvedViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ResolvedViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if(!User.IsInRole("Administrator"))
            {
                var name = User.Identity.Name;
                return View(await _unitOfWork.SupportTicketRepository.Get(filter: x => x.Creator == name && x.IsResolved == true, orderBy: x => x.OrderByDescending(x => x.DateCreated), pageNumber: 1, pageSize: int.MaxValue));
            }
            return View(await _unitOfWork.SupportTicketRepository.Get(filter: x => x.IsResolved == true, orderBy: x => x.OrderByDescending(x => x.DateCreated), pageNumber: 1, pageSize: int.MaxValue));
        }
    }
}