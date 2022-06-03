using System.Threading.Tasks;
using ProjectX.Models;

namespace ProjectX.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private GenericRepository<Discussion> discussionRepository;
        private GenericRepository<Reply> replyRepository;
        private GenericRepository<UserAction> userActionRepository;
        private GenericRepository<SupportTicket> supportTicketRepository;
        private GenericRepository<Category> categoryRepository;
        
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public GenericRepository<Discussion> DiscussionRepository
        {
            get
            {
                if (this.discussionRepository == null)
                {
                    this.discussionRepository = new GenericRepository<Discussion>(_context);
                }
                return discussionRepository;
            }
        }

        public GenericRepository<Reply> ReplyRepository
        {
            get
            {
                if (this.replyRepository == null)
                {
                    this.replyRepository = new GenericRepository<Reply>(_context);
                }
                return replyRepository;
            }
        }
        public GenericRepository<UserAction> UserActionRepository
        {
            get
            {
                if (this.userActionRepository == null)
                {
                    this.userActionRepository = new GenericRepository<UserAction>(_context);
                }
                return userActionRepository;
            }
        }
        public GenericRepository<SupportTicket> SupportTicketRepository
        {
            get
            {
                if (this.supportTicketRepository == null)
                {
                    this.supportTicketRepository = new GenericRepository<SupportTicket>(_context);
                }
                return supportTicketRepository;
            }
        }
        public GenericRepository<Category> CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new GenericRepository<Category>(_context);
                }
                return categoryRepository;
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
    }
}