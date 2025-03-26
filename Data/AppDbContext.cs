using Microsoft.EntityFrameworkCore;

namespace AI.Data
{
    public class UploadedFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string FileType { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
    }

}
