var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var openAi = builder.AddAzureOpenAI("openai");

var apiService = builder.AddProject<Projects.aspire_cv_analyzer_ApiService>("apiservice")
    .WithReference(openAi);

builder.AddProject<Projects.aspire_cv_analyzer_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
