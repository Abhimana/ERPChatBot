using Ollama.Core;
using Ollama.Core.Models;

namespace AI.Service
{
    public class ChatService : IChatService
    {
        private readonly IFileService _fileService;
        private readonly OllamaClient _ollamaClient;

        public ChatService(IFileService fileService, HttpClient httpClient)
        {
            _fileService = fileService;
            httpClient.BaseAddress ??= new Uri("http://localhost:11434");

            // Set timeout to 5 minutes
            httpClient.Timeout = TimeSpan.FromMinutes(5);

            _ollamaClient = new OllamaClient(httpClient);
        }

        public async Task<string> GetAnswerAsync(string question)
        {
            try
            {
                var contextText = await _fileService.ExtractTextFromFilesAsync();
                string prompt = $"Based on the following document text, answer the question.\n\nDocument:\n{contextText}\n\nQuestion: {question}";

                var response = await _ollamaClient.GenerateCompletionAsync(new GenerateCompletionOptions
                {
                    Model = "gemma:2b-instruct",
                    Prompt = prompt
                });

                return response?.Response ?? "No response from AI.";
            }
            catch (TaskCanceledException)
            {
                return "Request timed out. The server took too long to respond.";
            }
            catch (HttpRequestException ex)
            {
                return $"HTTP Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
        }
    }
}
