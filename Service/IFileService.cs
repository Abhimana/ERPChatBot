using AI.Data;

namespace AI.Service
{
    public interface IFileService
    {
        Task SaveFileAsync(IFormFile file);
        Task<List<UploadedFile>> GetFilesAsync();
        Task<string> ExtractTextFromFilesAsync();
    }

}
