using OpenAI;
using OpenAI.Chat;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddAzureOpenAIClient("openai");

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddTransient<IGenAiService, GenAiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapGet("/hello", () => {
    return "Hello world. This app built with .NET Aspire.";
});

app.MapGet("/summary", (OpenAIClient openAiClient) =>
{
    var chatClient = openAiClient.GetChatClient("my-gpt-35-turbo");
    ChatCompletion completion = chatClient.CompleteChat([
        new SystemChatMessage("You are a personal assistant for company to provide summery of cv"),
        new UserChatMessage("What is the meaning of life?")
        ]);

    var response = completion.Content[0].Text;

    return response;
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
