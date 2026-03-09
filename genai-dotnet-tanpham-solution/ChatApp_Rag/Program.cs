using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI;
using ChatApp_Rag.Components;
using ChatApp_Rag.Services;
using ChatApp_Rag.Services.Ingestion;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Lấy API Key của OpenAI từ cấu hình (User Secrets hoặc appsettings.json)
// Sử dụng command line: dotnet user-secrets set OpenAI:ApiKey YOUR-OPENAI-API-KEY
var openAiApiKey = builder.Configuration["OpenAI:ApiKey"]
    ?? throw new InvalidOperationException("Missing configuration: OpenAI:ApiKey.");

// Khởi tạo client chuẩn của OpenAI (Không cần ghi đè Endpoint như bản GitHub Models)
var openAiClient = new OpenAIClient(openAiApiKey);

// Khởi tạo Chat Client và Embedding Generator bằng model của OpenAI
var chatClient = openAiClient.GetChatClient("gpt-4o-mini").AsIChatClient();
var embeddingGenerator = openAiClient.GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

var vectorStorePath = Path.Combine(AppContext.BaseDirectory, "vector-store.db");
var vectorStoreConnectionString = $"Data Source={vectorStorePath}";
builder.Services.AddSqliteVectorStore(_ => vectorStoreConnectionString);
builder.Services.AddSqliteCollection<string, IngestedChunk>(IngestedChunk.CollectionName, vectorStoreConnectionString);

builder.Services.AddSingleton<DataIngestor>();
builder.Services.AddSingleton<SemanticSearch>();
builder.Services.AddKeyedSingleton("ingestion_directory", new DirectoryInfo(Path.Combine(builder.Environment.WebRootPath, "Data")));

// Đăng ký ChatClient và EmbeddingGenerator vào Dependency Injection
builder.Services.AddChatClient(chatClient).UseFunctionInvocation().UseLogging();
builder.Services.AddEmbeddingGenerator(embeddingGenerator);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();