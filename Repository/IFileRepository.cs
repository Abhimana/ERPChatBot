using AI.Data;

namespace AI.Repository
{
    public interface IFileRepository
    {
        Task SaveFileAsync(UploadedFile file);
        Task<List<UploadedFile>> GetFilesAsync();
    }
}
