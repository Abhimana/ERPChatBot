using AI.Service;
using Microsoft.AspNetCore.Mvc;

namespace AI.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileUploadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        [Route("uploadfile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please Upload file.");

            await _fileService.SaveFileAsync(file);
            return Ok("File uploaded successfully.");
        }

        [HttpGet]
        [Route("getallfiles")]
        public async Task<IActionResult> GetFiles()
        {
            var files = await _fileService.GetFilesAsync();
            return Ok(files);
        }
    }

}
