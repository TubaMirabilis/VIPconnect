using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectX.Data;
using ProjectX.Models;
using ProjectX.ViewModels;
using Tweetinvi;

namespace ProjectX.Controllers
{
    public class DiscussionsController : Controller
    {
        private readonly TwitterClient _twitterClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiscussionsController(TwitterClient twitterClient, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _twitterClient = twitterClient;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber, string category, string searchPhrase)
        {
            //Page number cannot be zero or negative.
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            var pageSize = 10;
            IEnumerable<Discussion> discussions;
            var count = 0;
            if (!String.IsNullOrEmpty(category))
            {
                //A big heading announces which category is being indexed:
                ViewBag.Category = category;
                //Get the discussions for the specific category ordered with most recent contribution at index 0:
                discussions = await _unitOfWork.DiscussionRepository.Get(filter: x => x.Category == category, orderBy: x => x.OrderByDescending(x => x.DateCreated), includeProperties: "Replies", pageNumber: pageNumber, pageSize: pageSize);
                //Using the repository Count method:
                count = await _unitOfWork.DiscussionRepository.Count(filter: x => x.Category == category);
            }
            else if (!String.IsNullOrEmpty(searchPhrase))
            {
                //The user used the search form on the homepage; remind them what they searched for:
                ViewBag.Category = $"Search Results for '{searchPhrase}'";
                discussions = await _unitOfWork.DiscussionRepository.Get(includeProperties: "Replies", filter: x => x.Title.Contains(searchPhrase) || x.Content.Contains(searchPhrase), orderBy: x => x.OrderByDescending(x => x.DateCreated), pageNumber: pageNumber, pageSize: pageSize);
                count = await _unitOfWork.DiscussionRepository.Count(filter: x => x.Title.Contains(searchPhrase) || x.Content.Contains(searchPhrase));
            }
            else
            {
                //The user just wants to see every discussion starting with the most recent:
                ViewBag.Category = "Discussions";
                discussions = await _unitOfWork.DiscussionRepository.Get(orderBy: x => x.OrderByDescending(x => x.DateCreated), includeProperties: "Replies", pageNumber: pageNumber, pageSize: pageSize);
                count = await _unitOfWork.DiscussionRepository.Count();
            }
            //Calculate the number of pages:
            var lastPage = (int)Math.Ceiling((double)count / pageSize) == 0 ? 1 : (int)Math.Ceiling((double)count / pageSize);
            //Prevent access to pages that don't exist:
            if(lastPage < pageNumber)
            {
                return RedirectToAction("Index", new { searchPhrase, category, pageNumber = 1 } );
            }
            if (discussions == null)
            {
                //After processing the parameters, there's nothing to index.
                return View(new IndexViewModel { Discussions = Enumerable.Empty<Discussion>() });
            }
            foreach (var item in discussions)
            {
                //Getting the discussion creator's avatar if there is one:
                item.Image = await GetImageAsync(item.Creator);
            }
            //Is the user browsing the final page of the index?
            var hasMore = pageNumber < lastPage ? true : false;
            var vm = new IndexViewModel
            {
                PageNumber = pageNumber,
                Category = category,
                PageCount = lastPage,
                PageNumbers = PageNumbers(pageNumber, lastPage),
                HasMore = hasMore,
                Discussions = discussions
            };
            return View(vm);
        }
        
        [Authorize]
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
            var discussion = await _unitOfWork.DiscussionRepository.GetById(reply.DiscussionId);
            if (ModelState.IsValid)
            {
                discussion.Replies = discussion.Replies ?? new List<Reply>();
                var x = new Reply
                {
                    Id = Guid.NewGuid(),
                    Content = reply.Content,
                    DateCreated = DateTime.UtcNow,
                    DiscussionId = reply.DiscussionId,
                    Respondent = User.Identity.Name,
                };
                discussion.Replies.Add(x);
                _unitOfWork.DiscussionRepository.Edit(discussion);
                await _unitOfWork.SaveChangesAsync();
                //Logging the details of this action:
                var userAction = new UserAction
                {
                    Id = Guid.NewGuid(),
                    Details = "Replied to discussion",
                    DiscussionId = discussion.Id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    ReplyId = x.Id,
                    LoggedAt = DateTime.UtcNow
                };
                _unitOfWork.UserActionRepository.Insert(userAction);
                await _unitOfWork.SaveChangesAsync();
            }
            //All done, showing the discussion's details page with the newly appended comment/reply:
            return RedirectToAction("Details", new { id = discussion.Id });
        }
        
        public async Task<IActionResult> Details(Guid? id, int pageNumber)
        {
            //Page number cannot be zero or negative.
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            var pageSize = 10;
            if (id == null)
            {
                return NotFound();
            }
            //Using the repository GetById method:
            var discussion = await _unitOfWork.DiscussionRepository.GetById(id, includeProperties: "Likes");
            if (discussion == null)
            {
                return NotFound();
            }
            //There will be an empty collection of replies if there are no genuine ones:
            var replies = await _unitOfWork.ReplyRepository.Get(filter: x => x.DiscussionId == id, orderBy: x => x.OrderBy(x => x.DateCreated), pageNumber: pageNumber, pageSize: pageSize)
                            ?? Enumerable.Empty<Reply>();
            //Get an src for the creator's avatar if they're using their external Twitter login:
            discussion.Image = await GetImageAsync(discussion.Creator);
            //Getting avatars for each person who made a reply on this discussion:
            foreach (var r in replies)
            {
                r.Image = await GetImageAsync(r.Respondent);
            }
            //Using the repository Count method:
            var count = await _unitOfWork.ReplyRepository.Count(filter: x => x.DiscussionId == id);
            //Calculate how many pages of replies:
            var lastPage = replies.Any() ? (int)Math.Ceiling((double)count / pageSize) : 1;
            //Trying to access a page that doesn't exist kicks the user back to page 1:
            if(lastPage < pageNumber)
            {
                return RedirectToAction("Details", new { id, pageNumber = 1 } );
            }
            //Is the user browsing the final page of replies?
            var hasMore = pageNumber < lastPage ? true : false;
            var vm = new DetailsViewModel
            {
                PageNumber = pageNumber,
                PageCount = lastPage,
                PageNumbers = PageNumbers(pageNumber, lastPage),
                HasMore = hasMore,
                Discussion = discussion,
                Replies = replies
            };
            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            if(User.IsInRole("Banned"))
            {
                return Forbid();
            }
            var categories = await _unitOfWork.CategoryRepository.Get(pageSize: int.MaxValue);
            var viewmodel = new CreateDiscussionViewModel
            {
                Categories = new List<SelectListItem>()
            };
            foreach (var c in categories)
            {
                viewmodel.Categories.Add(new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                });
            }
            return View(viewmodel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Creator,Title,DateCreated,Category,Content")] Discussion discussion)
        {
            if(User.IsInRole("Banned"))
            {
                return Forbid();
            }
            var categories = await _unitOfWork.CategoryRepository.Get(pageSize: int.MaxValue);
            var viewmodel = new CreateDiscussionViewModel
            {
                Categories = new List<SelectListItem>()
            };
            foreach (var c in categories)
            {
                viewmodel.Categories.Add(new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                });
            }
            if (ModelState.IsValid)
            {
                discussion.Id = Guid.NewGuid();
                discussion.Creator = User.Identity.Name;
                discussion.CreatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                discussion.DateCreated = DateTime.UtcNow;
                //Using the repository Insert method:
                _unitOfWork.DiscussionRepository.Insert(discussion);
                //Logging the successful creation of a discussion:
                var userAction = new UserAction
                {
                    Id = Guid.NewGuid(),
                    Details = "Started discussion",
                    DiscussionId = discussion.Id,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    LoggedAt = DateTime.UtcNow
                };
                //Using the repository Insert method:
                _unitOfWork.UserActionRepository.Insert(userAction);
                await _unitOfWork.SaveChangesAsync();
                //Show the creator the discussion that they have made:
                return RedirectToAction("Details", new { discussion.Id, pageNumber = 1 });
            }
            //Show the create form until the user completes all of the required fields:
            return View(viewmodel);
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if(User.IsInRole("Banned"))
            {
                return Forbid();
            }
            if (id == null)
            {
                return NotFound();
            }
            //Using the repository's GetById method:
            var discussion = await _unitOfWork.DiscussionRepository.GetById(id);
            if (discussion == null)
            {
                return NotFound();
            }
            //Only the creator can edit the body or 'content' of a discussion:
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) == discussion.CreatorId)
            {
                return PartialView(discussion);
            }
            //Otherwise this happens:
            return Forbid();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Creator,CreatorId,Title,DateCreated,Category,Content")] Discussion discussion)
        {
            if(User.IsInRole("Banned"))
            {
                return Forbid();
            }
            if (id != discussion.Id)
            {
                return NotFound();
            }
            //Only the creator can edit the body or 'content' of a discussion:
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != discussion.CreatorId)
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.DiscussionRepository.Edit(discussion);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exists = await DiscussionExists(discussion.Id);
                    if (!exists)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //Bounce the creator/editor back to their discussion's details page:
                return RedirectToAction("Details", new { id = discussion.Id, pageNumber = 1 });
            }
            //If we're here then the creator/editor replaced their discussion's content with an empty string which was a bit stupid:
            return View(discussion);
        }

        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            //Using the repository's GetById method:
            var discussion = await _unitOfWork.DiscussionRepository.GetById(id);
            if(discussion == null)
            {
                return NotFound();
            }
            //You can't delete the Admin's discussion unless you are the Admin:
            if(discussion.Creator == "Admin" && User.FindFirstValue(ClaimTypes.Name) != "Admin")
            {
                return Forbid();
            }
            //Now that's out of the way, only the creator, a moderator or the supreme Admin may delete a discussion:
            if(User.FindFirstValue(ClaimTypes.NameIdentifier) == discussion.CreatorId || User.IsInRole("Administrator") || User.IsInRole("Moderator"))
            {
                //Using the repository Delete method:
                await _unitOfWork.DiscussionRepository.Delete(discussion.Id);
                var actions = await _unitOfWork.UserActionRepository.Get(filter: x => x.DiscussionId == discussion.Id, pageSize: int.MaxValue);
                foreach (var action in actions)
                {
                    if(action == null)
                    {
                        continue;
                    }
                    await _unitOfWork.UserActionRepository.Delete(action.Id);
                }
                await _unitOfWork.SaveChangesAsync();
                //Bounce the creator/moderator/Admin back to the homepage:
                return RedirectToAction("Index", "Home");
            }
            //Someone was trying to delete without delete privileges:
            return Forbid();
        }
        public async Task<IActionResult> DeleteReply(Guid id)
        {
            //Using the repository's GetById method:
            var reply = await _unitOfWork.ReplyRepository.GetById(id);
            //Only a moderator or the supreme Admin may delete a reply:
            if(User.IsInRole("Administrator") || User.IsInRole("Moderator"))
            {
                //Using the repository's Delete method:
                await _unitOfWork.ReplyRepository.Delete(reply.Id);
                var actions = await _unitOfWork.UserActionRepository.Get(filter: x => x.ReplyId == reply.Id, pageSize: int.MaxValue);
                foreach (var action in actions)
                {
                    if(action == null)
                    {
                        continue;
                    }
                    await _unitOfWork.UserActionRepository.Delete(action.Id);
                }
                await _unitOfWork.SaveChangesAsync();
                //Bounce back to the discussion's details page with the offending reply removed:
                return RedirectToAction("Details", new { id = reply.DiscussionId });
            }
            //Someone was trying to delete without delete privileges:
            return Forbid();
        }
        private async Task<bool> DiscussionExists(Guid id)
        {
            var all = await _unitOfWork.DiscussionRepository.Get();
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
        public IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        {
            //Trying to get some numbers within a symetrical span of the current page:
            int median = pageNumber;
            if(median < 3)
            {
                median = 3;
            }
            else if (median > pageCount - 2)
            {
                median = pageCount - 2;
            }
            for (int i = median - 2; i <= median + 2; i++)
            {
                yield return i;
            }
        }
    }
}