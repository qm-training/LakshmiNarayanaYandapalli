namespace WelfareWorkTracker.Infrastructure.Repository;
public class ComplaintImageRepository(WelfareWorkTrackerContext context) : IComplaintImageRepository
{
    private readonly WelfareWorkTrackerContext _context = context;

    public async Task<ComplaintImage> AddComplaintImageAsync(ComplaintImage complaintImage)
    {
        await _context.ComplaintImages.AddAsync(complaintImage);
        await _context.SaveChangesAsync();
        var addedImage = await _context.ComplaintImages.FindAsync(complaintImage.ComplaintImageId);
        return addedImage!;
    }

    public async Task<List<ComplaintImage>> GetAllComplaintImagesByComplaintIdAsync(int complaintId)
    {
        var images = await _context.ComplaintImages.Where(i => i.ComplaintId == complaintId).ToListAsync();
        return images;
    }

    public async Task<ComplaintImage?> GetComplaintImageByIdAsync(int id)
    {
        var complaintImage = await _context.ComplaintImages.FindAsync(id);
        return complaintImage;
    }

    public async Task RemoveComplaintImageAsync(ComplaintImage complaintImage)
    {
        _context.ComplaintImages.Remove(complaintImage);
        await _context.SaveChangesAsync();
    }
}