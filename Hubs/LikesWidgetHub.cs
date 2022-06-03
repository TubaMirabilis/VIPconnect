using Microsoft.AspNetCore.SignalR;
using ProjectX.Data;
using ProjectX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.Hubs
{
    public class LikesWidgetHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        public LikesWidgetHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task SetLike(Guid id)
        {
            var discussion = await _unitOfWork.DiscussionRepository.GetById(id, includeProperties: "Likes");
            discussion.Likes = discussion.Likes ?? new List<DiscussionLike>();
            var temp = discussion.Likes.Where(x => x.IpAddress == this.Context.GetHttpContext().Connection.RemoteIpAddress.ToString()).FirstOrDefault();
            if(temp != null)
            {
                discussion.Likes.Remove(temp);
            }
            else
            {
                var x = new DiscussionLike
                {
                    Id = Guid.NewGuid(),
                    IpAddress = this.Context.GetHttpContext().Connection.RemoteIpAddress.ToString(),
                };
                discussion.Likes.Add(x);
            }
            _unitOfWork.DiscussionRepository.Edit(discussion);
            await _unitOfWork.SaveChangesAsync();
            var numOfLikes = discussion.Likes.Where(x => x.DiscussionId == id).Count();
            await Clients.All.SendAsync("ReceiveMessage", numOfLikes, id);
        }
    }
}