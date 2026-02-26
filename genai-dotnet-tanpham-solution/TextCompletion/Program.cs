
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using System.Text.Json.Serialization;

// get credentials from user secrets
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

// create a chat client
IChatClient client = new OpenAIClient(credential, options).GetChatClient("gpt-4o-mini").AsIChatClient();

#region Hoàn thành cơ bản

//// send prompt and get response
//string prompt = "What is AI ? explain max 20 word";
//Console.WriteLine($"user >>> {prompt}");

//ChatResponse response = await client.GetResponseAsync(prompt);

//Console.WriteLine($"assistant >>> {response}");
//Console.WriteLine($"Tokens used: in={response.Usage?.InputTokenCount}, out={response.Usage?.OutputTokenCount}");

#endregion

#region Dạng Streaming

//string prompt = "What is AI ? explain max 200 word";
//Console.WriteLine($"user >>> {prompt}");

//var responseStream = client.GetStreamingResponseAsync(prompt);
//await foreach (var message in responseStream)
//{
//    Console.Write(message.Text);
//}

#endregion

#region Phân loại văn bản
Console.OutputEncoding = System.Text.Encoding.UTF8;
//var classificationPrompt = """
//Vui lòng phân loại các câu sau đây vào các danh mục: 
//- 'phàn nàn' 
//- 'góp ý' 
//- 'khen ngợi' 
//- 'khác'.

//1) "Tôi rất thích giao diện mới!"
//2) "Các bạn nên thêm chế độ ban đêm."
//3) "Khi tôi thử đăng nhập, hệ thống liên tục báo lỗi."
//4) "Ứng dụng này khá ổn."
//""";

//Console.WriteLine($"user >>> {classificationPrompt}");

//ChatResponse classificationResponse = await client.GetResponseAsync(classificationPrompt);

//Console.WriteLine($"assistant >>>\n{classificationResponse}");

#endregion

#region Tóm tắt văn bản

//var summaryPrompt = """
//Tóm tắt bài đăng trên blog sau đây bằng 1 câu ngắn gọn.:

//"Kiến trúc microservices ngày càng phổ biến để xây dựng các ứng dụng phức tạp, nhưng nó cũng đi kèm với chi phí phát sinh. Điều quan trọng là phải đảm bảo mỗi dịch vụ càng nhỏ và tập trung càng tốt, và nhóm phát triển cần đầu tư vào các quy trình CI/CD mạnh mẽ để quản lý việc triển khai và cập nhật. Giám sát đúng cách cũng rất cần thiết để duy trì độ tin cậy khi hệ thống phát triển."
//""";

//Console.WriteLine($"user >>> {summaryPrompt}");

//ChatResponse summaryResponse = await client.GetResponseAsync(summaryPrompt);

//Console.WriteLine($"assistant >>> \n{summaryResponse}");

#endregion

#region Phân tích cảm xúc

//var analysisPrompt = """
//        Bạn sẽ phân tích sắc thái cảm xúc của các đánh giá sản phẩm sau đây. 
//        Mỗi dòng là một bài đánh giá riêng biệt. Hãy xuất kết quả phân tích sắc thái của từng bài đánh giá dưới dạng danh sách có dấu đầu dòng (bulleted list), và sau đó đưa ra sắc thái tổng thể cho tất cả các bài đánh giá.

//        Tôi đã mua sản phẩm này và nó thật tuyệt vời. 
//        Tôi rất thích nó! 
//        Sản phẩm này thật tồi tệ. 
//        Tôi ghét nó. 
//        Tôi không chắc chắn lắm về sản phẩm này. Nó cũng tạm được. 
//        Tôi tìm thấy sản phẩm này nhờ xem các đánh giá khác. Nó dùng được một thời gian ngắn, rồi sau đó lại không hoạt động nữa.
//        """;

//Console.WriteLine($"user >>> {analysisPrompt}");

//ChatResponse responseAnalysis = await client.GetResponseAsync(analysisPrompt);

//Console.WriteLine($"assistant >>> \n{responseAnalysis}");

#endregion

#region Output có cấu trúc

//var carListings = new[]
//{
//    "Check out this stylish 2019 Toyota Camry. It has a clean title, only 40,000 miles on the odometer, and a well-maintained interior. The car offers great fuel efficiency, a spacious trunk, and modern safety features like lane departure alert. Minimum offer price: $18,000. Contact Metro Auto at (555) 111-2222 to schedule a test drive.",
//    "Lease this sporty 2021 Honda Civic! With only 10,000 miles, it includes a sunroof, premium sound system, and backup camera. Perfect for city driving with its compact size and great fuel mileage. Located in Uptown Motors, monthly lease starts at $250 (excl. taxes). Call (555) 333-4444 for more info.",
//    "A classic 1968 Ford Mustang, perfect for enthusiasts. The vehicle needs some interior restoration, but the engine runs smoothly. V8 engine, manual transmission, around 80,000 miles. This vintage gem is priced at $25,000. Contact Retro Wheels at (555) 777-8888 if you’re interested.",
//    "Brand new 2023 Tesla Model 3 for lease. Zero miles, fully electric, autopilot capabilities, and a sleek design. Monthly lease starts at $450. Clean lines, minimalist interior, top-notch performance. For more details, call EVolution Cars at (555) 999-0000.",
//    "Selling a 2015 Subaru Outback in good condition. 60,000 miles on it, includes all-wheel drive, heated seats, and ample cargo space for family getaways. Minimum offer price: $14,000. Contact Forrest Autos at (555) 222-1212 if you want a reliable adventure companion.",
//};

//foreach (var listingText in carListings)
//{
//    var response = await client.GetResponseAsync<CarDetails>(
//        $"""
//        Convert the following car listing into a JSON object matching this C# schema:
//        Condition: "New" or "Used"
//        Make: (car manufacturer)
//        Model: (car model)
//        Year: (four-digit year)
//        ListingType: "Sale" or "Lease"
//        Price: integer only
//        Features: array of short strings
//        TenWordSummary: exactly ten words to summarize this listing

//        Here is the listing:
//        {listingText}
//        """);

//    if (response.TryGetResult(out var info))
//    {
//        // Convert the CarDetails object to JSON for display
//        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(
//            info, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
//    }
//    else
//    {
//        Console.WriteLine("Response was not in the expected format.");
//    }
//}

//class CarDetails
//{
//    public required string Condition { get; set; }  // e.g. "New" or "Used"
//    public required string Make { get; set; }
//    public required string Model { get; set; }
//    public int Year { get; set; }
//    public CarListingType ListingType { get; set; }
//    public int Price { get; set; }
//    public required string[] Features { get; set; }
//    public required string TenWordSummary { get; set; }
//}

//[JsonConverter(typeof(JsonStringEnumConverter))]
//enum CarListingType { Sale, Lease }

#endregion

#region ChatApp

List<ChatMessage> chatHistory = new()
{
    new ChatMessage(ChatRole.System, """
                Bạn là một người đam mê đi bộ đường dài (hiking) thân thiện, chuyên giúp mọi người khám phá những cung đường thú vị trong khu vực của họ.
                Bạn sẽ tự giới thiệu bản thân khi chào hỏi lần đầu.
                Khi giúp đỡ mọi người, bạn luôn hỏi họ những thông tin sau để đưa ra các gợi ý đi bộ đường dài phù hợp:

                1. Địa điểm mà họ muốn đi bộ đường dài
                2. Mức độ khó (cường độ) mà họ mong muốn

                Sau khi nhận được các thông tin đó, bạn sẽ cung cấp 3 gợi ý về các cung đường gần đó với độ dài khác nhau. Bạn cũng sẽ chia sẻ một sự thật thú vị về thiên nhiên địa phương trên các cung đường này khi đưa ra gợi ý. Ở cuối câu trả lời, hãy hỏi xem bạn có thể giúp gì thêm cho họ không.
            """)
};

while (true)
{
    // Get user prompt and add to chat history
    Console.WriteLine("Your prompt:");
    var userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    // Stream the AI response and add to chat history
    Console.WriteLine("AI Response:");
    var response = "";
    await foreach (var item in
        client.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;
    }
    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    Console.WriteLine();
}

#endregion