namespace WelfareWorkTracker.Infrastructure.Repository;
public class CommentRepository(WelfareWorkTrackerContext context) : ICommentRepository
{
    private readonly WelfareWorkTrackerContext _context = context;

    public async Task<Comment> AddCommentByIdAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return comment;
    }
    public async Task<Comment?> GetCommentByIdAsync(int commentId)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
        return comment;
    }

    public async Task<List<Comment>> GetCommentsByIdAsync(int? complaintId, int? dailyComplaintId, int userId)
    {
        var commentsQuery = _context.Comments.AsQueryable();

        // Filter by ComplaintId or DailyComplaintId
        if (complaintId.HasValue)
        {
            commentsQuery = commentsQuery.Where(c => c.ComplaintId == complaintId.Value);
        }
        else if (dailyComplaintId.HasValue)
        {
            commentsQuery = commentsQuery.Where(c => c.DailyComplaintId == dailyComplaintId.Value);
        }

        // Fetch and sort comments
        var comments = await commentsQuery
            .OrderByDescending(c => c.UserId == userId) // User's comments first
            .ThenByDescending(c => c.DateCreated) // Then by creation date
            .ToListAsync();

        return comments;
    }
    public async Task<Comment> UpdateCommentByIdAsync(Comment comment)
    {
        var updatedComment = _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
        return updatedComment.Entity;
    }

    public async Task<Comment> DeleteCommentAsync(Comment comment)
    {
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return comment;
    }
}
