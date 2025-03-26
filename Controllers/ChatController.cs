using AI.Service;
using Microsoft.AspNetCore.Mvc;

namespace AI.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        [Route("virtualassistant")]
        public async Task<IActionResult> AskQuestion(string question)
        {
            var answer = await _chatService.GetAnswerAsync(question);
            return Ok(new { question, answer });
        }
    }

}
