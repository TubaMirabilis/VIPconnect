using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Data;
using ProjectX.Models;

namespace ProjectX.Controllers
{
    [Authorize(Roles = "Administrator, Moderator")]
    public class RolesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public RolesController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        // Eventually it will be possible to batch process role changes through a Razor view.
        // public IActionResult Update()
        // {
        //     return View(new List<NameRolePair>());
        // }
        public async Task<IActionResult> UpdateSingle(string userName, string role, bool batchDelete, Guid? id = null)
        {
            if(User.IsInRole("Banned"))
            {
                return Forbid();
            }
            if(userName == "Admin")
            {
                //You can't change the roles of the supreme Admin.
                return RedirectToAction("Details", "Discussions", new { pageNumber = 1, id });
            }
            //I need an ApplicationUser object:
            var user = await _userManager.FindByNameAsync(userName);
            if(role == "Unban")
            {
                await _userManager.RemoveFromRoleAsync(user, "Banned");
                if(id == null)
                {
                    //The moderator probably executed this action from the Profiles/Details Razor view, kicking them back to the homepage:
                    return RedirectToAction("Index", "Home");
                }
                //The moderator probably executed this action from the Discussions/Details Razor view, kicking them back to the same page:
                return RedirectToAction("Details", "Discussions", new { pageNumber = 1, id });
            }
            //Only the supreme Admin can unmod a moderator.
            if(role == "Unmod" && User.IsInRole("Administrator"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Moderator");
                if(id == null)
                {
                    //The supreme Admin probably executed this action from the Profiles/Details Razor view, kicking them back to the homepage:
                    return RedirectToAction("Index", "Home");
                }
                //The supreme Admin probably executed this action from the Discussions/Details Razor view, kicking them back to the same page:
                return RedirectToAction("Details", "Discussions", new { pageNumber = 1, id });
            }
            if(role == "Banned")
            {
                //If the person we're trying to ban is a moderator we want to initially remove their moderator privileges:
                if(await _userManager.IsInRoleAsync(user, "Moderator"))
                {
                    await _userManager.RemoveFromRoleAsync(user, "Moderator");
                }
                //The moderator or admin can choose to delete a user's contributions when issuing a ban:
                if(batchDelete)
                {
                    await BatchDeleteAsync(userName);
                }
            }
            //Having finally nagotiated the guard clauses, we can do something straight-forward:
            await _userManager.AddToRoleAsync(user, role);
            if(id == null)
            {
                //The supreme Admin probably executed this action from the Profiles/Details Razor view, kicking them back to the homepage:
                return RedirectToAction("Index", "Home");
            }
            if(batchDelete)
            {
                return RedirectToAction("Index", "Home");

            }
            //The moderator probably executed this action from the Discussions/Details Razor view, kicking them back to the same page:
            return RedirectToAction("Details", "Discussions", new { pageNumber = 1, id });
        }
        async Task BatchDeleteAsync(string userName)
        {
            var discussions = await _unitOfWork.DiscussionRepository.Get(filter: x => x.Creator == userName, pageSize: int.MaxValue)
                                ?? Enumerable.Empty<Discussion>();
            foreach (var d in discussions)
            {
                if(d != null)
                {
                    var id = d.Id;
                    await _unitOfWork.DiscussionRepository.Delete(id);
                    var actions = await _unitOfWork.UserActionRepository.Get(filter: x => x.DiscussionId == d.Id, pageSize: int.MaxValue);
                    foreach (var action in actions)
                    {
                        if(action == null)
                        {
                            continue;
                        }
                        await _unitOfWork.UserActionRepository.Delete(action.Id);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            var replies = await _unitOfWork.ReplyRepository.Get(filter: x => x.Respondent == userName, pageSize: int.MaxValue)
                            ?? Enumerable.Empty<Reply>();
            foreach (var r in replies)
            {
                if(r != null)
                {
                    var id = r.Id;
                    await _unitOfWork.ReplyRepository.Delete(id);
                    var actions = await _unitOfWork.UserActionRepository.Get(filter: x => x.ReplyId == r.Id, pageSize: int.MaxValue);
                    foreach (var action in actions)
                    {
                        if(action == null)
                        {
                            continue;
                        }
                        await _unitOfWork.UserActionRepository.Delete(action.Id);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }
    }
}