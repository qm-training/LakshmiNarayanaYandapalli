using Microsoft.EntityFrameworkCore;
using WelfareWorkTracker.Core.Contracts.Repository;
using WelfareWorkTracker.Core.Models;
using WelfareWorkTracker.Infrastructure.Data;

namespace WelfareWorkTracker.Infrastructure.Repository
{
    public class CommentRepository(WelfareWorkTrackerContext context) : ICommentRepository
    {
        private readonly WelfareWorkTrackerContext _context = context;
        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteCommentAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            var count = await _context.SaveChangesAsync();
            return count > 0;
        }

        public async Task<Comment?> GetCommentByCommentIdAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            return comment;
        }

        public async Task<List<Comment>> GetCommentsByComplaintIdAsync(int complaintId)
        {
            var comments = await _context.Comments.Where(c => c.ComplaintId == complaintId).ToListAsync();
            return comments;
        }

        public async Task<List<Comment>> GetCommentsByUserIdAsync(int userId)
        {
            var comments = await _context.Comments.Where(c => c.UserId == userId).ToListAsync();
            return comments;
        }

        public async Task<Comment> UpdateCommentAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
    }
}
