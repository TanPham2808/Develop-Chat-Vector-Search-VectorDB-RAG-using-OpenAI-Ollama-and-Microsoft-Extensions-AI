using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Text.Json.Serialization;

IChatClient client =
    new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2");

Console.OutputEncoding = System.Text.Encoding.UTF8;

#region Hoàn thành cơ bản

//string prompt = "What is AI ? explain max 20 word. Reply for me by Vietnamese";
//Console.WriteLine($"user >>> {prompt}");

//ChatResponse response = await client.GetResponseAsync(prompt);

//Console.WriteLine($"assistant >>> {response}");
//Console.WriteLine($"Tokens used: in={response.Usage?.InputTokenCount}, out={response.Usage?.OutputTokenCount}");

#endregion

#region Phân loại văn bản
//var classificationPrompt = """
//    Vui lòng phân loại các câu sau đây vào các danh mục: 
//    - 'phàn nàn' 
//    - 'góp ý' 
//    - 'khen ngợi' 
//    - 'khác'.

//    1) "Tôi rất thích giao diện mới!"
//    2) "Các bạn nên thêm chế độ ban đêm."
//    3) "Khi tôi thử đăng nhập, hệ thống liên tục báo lỗi."
//    4) "Ứng dụng này khá ổn."
//    """;

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

var carListings = new[]
{
  "Hãy xem qua chiếc Toyota Camry 2019 phong cách này. Xe có giấy tờ hợp lệ (clean title), đồng hồ công-tơ-mét chỉ mới 40.000 dặm, và nội thất được bảo dưỡng tốt. Chiếc xe mang lại hiệu quả tiết kiệm nhiên liệu tuyệt vời, cốp xe rộng rãi và các tính năng an toàn hiện đại như cảnh báo chệch làn đường. Giá đề nghị tối thiểu: 18.000 USD. Liên hệ Metro Auto qua số (555) 111-2222 để đặt lịch lái thử.",
  "Thuê chiếc Honda Civic 2021 đậm chất thể thao này! Chỉ với 10.000 dặm, xe có cửa sổ trời, hệ thống âm thanh cao cấp và camera lùi. Hoàn hảo để lái trong thành phố với kích thước nhỏ gọn và mức tiêu hao nhiên liệu cực tốt. Xe nằm tại Uptown Motors, giá thuê hàng tháng bắt đầu từ 250 USD (chưa bao gồm thuế). Gọi (555) 333-4444 để biết thêm thông tin.",
  "Một chiếc Ford Mustang 1968 cổ điển, hoàn hảo cho những người đam mê. Chiếc xe cần phục hồi một chút nội thất, nhưng động cơ vẫn chạy êm ái. Động cơ V8, hộp số sàn, đã chạy khoảng 80.000 dặm. 'Viên ngọc' cổ điển này có giá 25.000 USD. Liên hệ Retro Wheels qua số (555) 777-8888 nếu bạn quan tâm.",
  "Cho thuê xe Tesla Model 3 2023 mới tinh. Chưa lăn bánh (zero miles), thuần điện, có khả năng lái tự động (autopilot) và thiết kế kiểu dáng đẹp. Giá thuê hàng tháng bắt đầu từ 450 USD. Đường nét gọn gàng, nội thất tối giản, hiệu suất đỉnh cao. Để biết thêm chi tiết, hãy gọi EVolution Cars qua số (555) 999-0000.",
  "Bán một chiếc Subaru Outback 2015 trong tình trạng tốt. Xe đã chạy 60.000 dặm, bao gồm hệ dẫn động 4 bánh toàn thời gian (AWD), ghế sưởi và không gian chứa đồ rộng rãi cho những chuyến đi xa của gia đình. Giá đề nghị tối thiểu: 14.000 USD. Liên hệ Forrest Autos qua số (555) 222-1212 nếu bạn muốn có một người bạn đồng hành đáng tin cậy trong những chuyến phiêu lưu."
};

foreach (var listingText in carListings)
{
    var response = await client.GetResponseAsync<CarDetails>(
      $"""
        Chuyển đổi thông tin đăng bán xe dưới đây thành một đối tượng JSON khớp với cấu trúc C# sau:
        Condition: "New" (Mới) hoặc "Used" (Cũ)
        Make: (hãng sản xuất xe)
        Model: (dòng xe)
        Year: (năm sản xuất gồm 4 chữ số)
        ListingType: "Sale" (Bán) hoặc "Lease" (Cho thuê)
        Price: chỉ ghi số nguyên
        Features: mảng các chuỗi ngắn (chứa các tính năng của xe)
        TenWordSummary: dùng chính xác 10 từ để tóm tắt thông tin này

        Dưới đây là thông tin xe:
        {listingText}
        """);

    if (response.TryGetResult(out var info))
    {
        // Convert the CarDetails object to JSON for display
        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(
            info, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
    }
    else
    {
        Console.WriteLine("Response was not in the expected format.");
    }
}

class CarDetails
{
    public required string Condition { get; set; }  // e.g. "New" or "Used"
    public required string Make { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public CarListingType ListingType { get; set; }
    public int Price { get; set; }
    public required string[] Features { get; set; }
    public required string TenWordSummary { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
enum CarListingType { Sale, Lease }

#endregion

#region ChatApp

//List<ChatMessage> chatHistory = new()
//{
//    new ChatMessage(ChatRole.System, """
//                Bạn là một người đam mê đi bộ đường dài (hiking) thân thiện, chuyên giúp mọi người khám phá những cung đường thú vị trong khu vực của họ.
//                Bạn sẽ tự giới thiệu bản thân khi chào hỏi lần đầu.
//                Khi giúp đỡ mọi người, bạn luôn hỏi họ những thông tin sau để đưa ra các gợi ý đi bộ đường dài phù hợp:

//                1. Địa điểm mà họ muốn đi bộ đường dài
//                2. Mức độ khó (cường độ) mà họ mong muốn

//                Sau khi nhận được các thông tin đó, bạn sẽ cung cấp 3 gợi ý về các cung đường gần đó với độ dài khác nhau. Bạn cũng sẽ chia sẻ một sự thật thú vị về thiên nhiên địa phương trên các cung đường này khi đưa ra gợi ý. Ở cuối câu trả lời, hãy hỏi xem bạn có thể giúp gì thêm cho họ không.
//            """)
//};

//while (true)
//{
//    // Get user prompt and add to chat history
//    Console.WriteLine("Your prompt:");
//    var userPrompt = Console.ReadLine();
//    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

//    // Stream the AI response and add to chat history
//    Console.WriteLine("AI Response:");
//    var response = "";
//    await foreach (var item in
//        client.GetStreamingResponseAsync(chatHistory))
//    {
//        Console.Write(item.Text);
//        response += item.Text;
//    }
//    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
//    Console.WriteLine();
//}

#endregion