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
    var content = @"
### Contact Information
* **Name:** John Doe
* **Email:** [email address removed]
* **Phone:** +1 (123) 456-7890
* **LinkedIn:** [Your LinkedIn Profile]

### Summary
Experienced Software Engineer with a strong foundation in [Programming Languages, e.g., Python, Java, C++]. Proficient in [Technologies, e.g., Django, React, SQL]. Passionate about building scalable and efficient software solutions.

### Skills
* **Technical Skills:**
    * Programming Languages: Python, Java, C++
    * Frameworks and Libraries: Django, React, TensorFlow
    * Databases: MySQL, PostgreSQL
    * Cloud Platforms: AWS, GCP
* **Soft Skills:**
    * Problem-solving
    * Teamwork
    * Communication
    * Time management
";

    var chatClient = openAiClient.GetChatClient("my-gpt-35-turbo");
    ChatCompletion completion = chatClient.CompleteChat([
        new SystemChatMessage("You are a highly skilled personal assistant tasked with summarizing CVs for a company. Please provide a concise and informative summary, highlighting the candidate's key skills, experiences, and qualifications. Focus on the most relevant information for the role being considered."),
        new UserChatMessage($"Here is the CV I want you to summarize: {content}")
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
