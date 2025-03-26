using AI.Data;
using Microsoft.EntityFrameworkCore;

namespace AI.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly AppDbContext _context;
        public FileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveFileAsync(UploadedFile file)
        {
            _context.UploadedFiles.Add(file);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UploadedFile>> GetFilesAsync()
        {
            return await _context.UploadedFiles.ToListAsync();
        }
    }

}
