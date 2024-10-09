

using OpenAI;

namespace Services
{
    public class GenAiService : IGenAiService
    {
        public async Task<string> GetOpenAIResponseAsync(string text)
        {
            return "Hello from GenAiService";
        }
    }
}
