using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// get credentials from user secrets
var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

// .UseFunctionInvocation(): RẤT QUAN TRỌNG. Lệnh này cho phép thư viện tự động xử lý các yêu cầu gọi hàm (function calling) từ AI.
IChatClient client =
    new ChatClientBuilder(new OpenAIClient(credential, options)
    .GetChatClient("gpt-4o-mini").AsIChatClient())
    .UseFunctionInvocation()
    .Build();


// Cấu hình các tuỳ chọn cho cuộc trò chuyện, bao gồm việc cung cấp công cụ (Tools) cho AI sử dụng.
var chatOptions = new ChatOptions
{
    // Tạo một tool tên là "get_current_weather".
    Tools = [AIFunctionFactory.Create((string location, string unit) =>
    {
        var temperature = Random.Shared.Next(5, 20);
        var conditions = Random.Shared.Next(0, 1) == 0 ? "sunny" : "rainy";

        return $"The weather is {temperature} degrees C and {conditions}.";
    },
    "get_current_weather", // Tên của công cụ để AI nhận diện.
    "Get the current weather in a given location")] // Mô tả để AI hiểu công cụ này dùng để làm gì và khi nào nên gọi.
};

// Bắt đầu bằng một System Prompt để định hình tính cách cho AI.
List<ChatMessage> chatHistory = [new(ChatRole.System, """
    You are a hiking enthusiast who helps people discover fun hikes in their area. 
    You are upbeat and friendly.
    """)];

// Người dùng hỏi về một tuyến đường leo núi ở Istanbul VÀ hỏi thêm thời tiết hiện tại.
chatHistory.Add(new(ChatRole.User, """
    I live in Istanbul and I'm looking for a moderate intensity hike. 
    What's the current weather like? 
    """));

Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");


// Gửi toàn bộ lịch sử và tuỳ chọn tới AI để lấy câu trả lời.
// Quá trình diễn ra ngầm nhờ .UseFunctionInvocation():
// - AI nhận thấy người dùng hỏi thời tiết.
// - AI tạm dừng trả lời và yêu cầu chạy hàm "get_current_weather" với tham số location là "Istanbul".
// - Hàm C# ở trên tự động được chạy ngầm, tạo ra nhiệt độ giả lập, và gửi kết quả ngược lại cho AI.
// - AI dùng kết quả thời tiết đó để tạo ra câu trả lời tự nhiên cuối cùng.
ChatResponse response = await client.GetResponseAsync(chatHistory, chatOptions);

chatHistory.Add(new(ChatRole.Assistant, response.Text));

Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");