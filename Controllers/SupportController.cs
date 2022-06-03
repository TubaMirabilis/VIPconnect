using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Models;
using Tweetinvi;

namespace ProjectX.Controllers
{
    [Authorize]
    public class SupportController : Controller
    {
        private readonly TwitterClient _twitterClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public SupportController(TwitterClient twitterClient, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _twitterClient = twitterClient;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Comment(Reply reply)
        {
            //Do nothing if the reply lacks content:
            if (!ModelState.IsValid || string.IsNullOrEmpty(reply.Content))
            {
                return RedirectToAction("Details", new { id = reply.DiscussionId });
            }
            if(User.IsInRole("Banned"))
            {
                return Forbid();
            }
            var ticket = await _unitOfWork.SupportTicketRepository.GetById(reply.SupportTicketId);
            if (ModelState.IsValid)
            {
                ticket.Replies = ticket.Replies ?? new List<Reply>();
                var x = new Reply
                {
                    Id = Guid.NewGuid(),
                    Content = reply.Content,
                    DateCreated = DateTime.UtcNow,
                    Respondent = User.Identity.Name,
                    SupportTicketId = reply.SupportTicketId,
                };
                ticket.Replies.Add(x);
                _unitOfWork.SupportTicketRepository.Edit(ticket);
                await _unitOfWork.SaveChangesAsync();
            }
            //All done, showing the discussion's details page with the newly appended comment/reply:
            return RedirectToAction("Details", new { id = ticket.Id });
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupportTicket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Id = Guid.NewGuid();
                ticket.Creator = User.Identity.Name;
                ticket.DateCreated = DateTime.UtcNow;
                ticket.IsResolved = false;
                //Using the repository Insert method:
                _unitOfWork.SupportTicketRepository.Insert(ticket);
                //Logging the successful creation of a support ticket:
                var userAction = new UserAction
                {
                    Id = Guid.NewGuid(),
                    Details = "Started Support Ticket",
                    TicketId = ticket.Id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    LoggedAt = DateTime.UtcNow
                };
                //Using the repository Insert method:
                _unitOfWork.UserActionRepository.Insert(userAction);
                await _unitOfWork.SaveChangesAsync();
                //Show the creator the support ticket that they have made:
                return RedirectToAction("Details", new { ticket.Id });
            }
            //Show the create form until the user completes all of the required fields:
            return View();
        }
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Using the repository GetById method:
            var ticket = await _unitOfWork.SupportTicketRepository.GetById(id, includeProperties: "Replies");
            if (ticket == null)
            {
                return NotFound();
            }
            if(User.Identity.Name != ticket.Creator && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }
            ticket.Image = await GetImageAsync(ticket.Creator);
            if(ticket.Replies.Any())
            {
                foreach (var r in ticket.Replies)
                {
                    r.Image = await GetImageAsync(r.Respondent);
                }
            }
            return View(ticket);
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            //Using the repository's GetById method:
            var ticket = await _unitOfWork.SupportTicketRepository.GetById(id, includeProperties: "Replies");
            if(ticket == null)
            {
                return NotFound();
            }
            //Using the repository Delete method:
            await _unitOfWork.SupportTicketRepository.Delete(ticket.Id);
            var actions = await _unitOfWork.UserActionRepository.Get(filter: x => x.TicketId == ticket.Id, pageSize: int.MaxValue);
            foreach (var action in actions)
            {
                if(action == null)
                {
                    continue;
                }
                await _unitOfWork.UserActionRepository.Delete(action.Id);
            }
            await _unitOfWork.SaveChangesAsync();
            //Bounce the user back to the Support Centre homepage:
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> MarkAsResolved (Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ticket = await _unitOfWork.SupportTicketRepository.GetById(id);
            ticket.IsResolved = true;
            try
            {
                _unitOfWork.SupportTicketRepository.Edit(ticket);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await TicketExists(id);
                if (!exists)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //Bounce the user back to the Support Centre homepage:
            return RedirectToAction("Index");
        }
        private async Task<bool> TicketExists(Guid? id)
        {
            var all = await _unitOfWork.SupportTicketRepository.Get();
            return all.Any(e => e.Id == id);
        }
        public async Task<string> GetImageAsync(string user)
        {
            //To do this I need an ApplicationUser object:
            var query = await _userManager.FindByNameAsync(user);
            var claims = await _userManager.GetClaimsAsync(query);
            //Everyone with the "TwitterId" claim should have an avatar provided by Twitter:
            if(!claims.Any(c => c.Type == "TwitterId"))
            {
                //No claim
                return "";
            }
            else
            {
                return await _twitterClient.GetImageAsync(claims.FirstOrDefault(c => c.Type == "TwitterId").Value);
            }
        }
    }
}