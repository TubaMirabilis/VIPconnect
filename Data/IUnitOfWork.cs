using System.Threading.Tasks;
using ProjectX.Models;

namespace ProjectX.Data
{
    public interface IUnitOfWork
    {
        GenericRepository<Discussion> DiscussionRepository { get; }
        GenericRepository<Reply> ReplyRepository { get; }
        GenericRepository<UserAction> UserActionRepository { get; }
        GenericRepository<SupportTicket> SupportTicketRepository { get; }
        GenericRepository<Category> CategoryRepository { get; }
        Task<bool> SaveChangesAsync();
    }
}