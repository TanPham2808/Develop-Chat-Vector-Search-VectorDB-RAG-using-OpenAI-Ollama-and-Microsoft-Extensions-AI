using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;
using System.ClientModel;
using VectorSearch;

// Lấy credentials từ user secrets
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

// Tạo một embedding generator (ví dụ sử dụng mô hình text-embedding-3-small)
IEmbeddingGenerator<string, Embedding<float>> generator =
    new OpenAIClient(credential, options)
        .GetEmbeddingClient("openai/text-embedding-3-small")
        .AsIEmbeddingGenerator();

// Khởi tạo và nạp dữ liệu vào vector store
var vectorStore = new InMemoryVectorStore();

var moviesStore = vectorStore.GetCollection<int, Movie>("movies");

await moviesStore.EnsureCollectionExistsAsync();

foreach (var movie in MovieData.Movies)
{
    // Tạo embedding vector cho phần mô tả (description) của bộ phim
    movie.Vector = await generator.GenerateVectorAsync(movie.Description);

    // Thêm toàn bộ object movie vào movie collection của in-memory vector store
    await moviesStore.UpsertAsync(movie);
}

// 1- Tạo embedding cho query của người dùng (Vector hóa truy vấn của người dùng)
// 2- Thực hiện vectorized search (tìm kiếm bằng vector)
// 3- Trả về các records (bản ghi) kết quả

// Tạo embedding vector cho prompt của người dùng
var query = "I want to see family friendly movie";
//var query = "A science fiction movie about space travel";
var queryEmbedding = await generator.GenerateVectorAsync(query);

// Tìm kiếm trong knowledge store dựa trên prompt của người dùng
var searchResults = moviesStore.SearchAsync(queryEmbedding, top: 2);

// In ra các kết quả để xem cấu trúc dữ liệu trả về trông như thế nào
await foreach (var result in searchResults)
{
    Console.WriteLine($"Title: {result.Record.Title}");
    Console.WriteLine($"Description: {result.Record.Description}");
    Console.WriteLine($"Score: {result.Score}");
    Console.WriteLine();
}