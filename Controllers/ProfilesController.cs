using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Data;
using ProjectX.Models;
using ProjectX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;

namespace ProjectX.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly TwitterClient _twitterClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfilesController(TwitterClient twitterClient, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _twitterClient = twitterClient;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<IActionResult> Details(string user, int pageNumber)
        {
            //Page number cannot be zero or negative:
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            var pageSize = 10;
            //I need an ApplicationUser object:
            var query = await _userManager.FindByNameAsync(user);
            var id = await _userManager.GetUserIdAsync(query);
            //I'll be checking for the "TwitterID" claim later:
            var claims = await _userManager.GetClaimsAsync(query);
            var image = "";
            //Checking now:
            if(claims.Any(c => c.Type == "TwitterId"))
            {
                image = await _twitterClient.GetImageAsync(claims.FirstOrDefault(c => c.Type == "TwitterId").Value);
            }
            //Using the UserAction repository to get the user's contributions to the website:
            var contributions = await _unitOfWork.UserActionRepository.Get(filter: x => x.UserId == id && x.Details != "Started Support Ticket", orderBy: x => x.OrderByDescending(x => x.LoggedAt), pageNumber: pageNumber, pageSize: pageSize);
            if(contributions == null)
            {
                if(pageNumber == 1)
                {
                    //User hasn't generated any content for the website yet.
                    contributions = Enumerable.Empty<UserAction>();
                }
                if(pageNumber > 1)
                {
                    //Preventing access to pages that don't exist; kicking the observer back to page 1:
                    return RedirectToAction("Details", new { user, pageNumber = 1 } );
                }
            }
            //Shoving everything into a list of an abstract type, I'll cast everything back into it's original form in the view:
            var contributionsAsTextContent = new List<TextContent>();
            //I need to know which discussions the user has replied to:
            var repliedTo = new List<Discussion>();
            foreach (var c in contributions)
            {
                if (c.Details == "Replied to discussion")
                {
                    var reply = await _unitOfWork.ReplyRepository.GetById(c.ReplyId);
                    if(reply == null)
                    {
                        //reply was probably deleted by a moderator at some point.
                        continue;
                    }
                    contributionsAsTextContent.Add(reply);
                    repliedTo.Add(await _unitOfWork.DiscussionRepository.GetById(c.DiscussionId));
                }
                if (c.Details == "Started discussion")
                {
                    var discussion = await _unitOfWork.DiscussionRepository.GetById(c.DiscussionId);
                    if(discussion != null)
                    {
                        contributionsAsTextContent.Add(discussion);
                    }
                }
            }
            //Using the repository's Count method:
            int count = await _unitOfWork.UserActionRepository.Count(filter: x => x.UserId == id);
            //Calculate how many pages in the activity log:
            var lastPage = (int)Math.Ceiling((double)count / pageSize);
            //Is the observer browsing the final page of the activity log?
            var hasMore = pageNumber < lastPage ? true : false;
            var viewModel = new ProfileViewModel
            {
                User = query,
                Contributions = contributionsAsTextContent,
                Discussions = repliedTo,
                Image = image,
                PageNumber = pageNumber,
                PageCount = lastPage,
                PageNumbers = PageNumbers(pageNumber, lastPage),
                HasMore = hasMore,
            };
            return View(viewModel);
        }
        public IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        {
            //Trying to get some numbers within a symetrical span of the median:
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