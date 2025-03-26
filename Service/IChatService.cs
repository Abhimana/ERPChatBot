namespace AI.Service
{
    public interface IChatService
    {
        Task<string> GetAnswerAsync(string question);
    }

}
