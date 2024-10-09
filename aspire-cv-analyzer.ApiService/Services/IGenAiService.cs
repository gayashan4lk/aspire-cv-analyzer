namespace Services
{
    public interface IGenAiService
    {
        public Task<string> GetOpenAIResponseAsync(string text);
    }
}
