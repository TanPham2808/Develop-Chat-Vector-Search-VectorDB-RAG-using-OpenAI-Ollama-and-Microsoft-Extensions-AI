
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;
using System.ComponentModel;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

IChatClient chatClient = new OpenAIClient(credential, options)
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

string qualityReviewerAgentInstructions = """
    You are a multilingual translation quality reviewer.
    Check the translations for grammar accuracy, tone consistency, and cultural fit
    compared to the original English text.

    Give a brief summary with a quality rating (Excellent / Good / Needs Review).

    Example out:
    Quality: Excellent
    Feedback: Accurate translation, friendly tone preserved, minor punctuation tweaks only.
    """;

string summaryAgentInstructions = """
    You are a localization summary assistant.
    Summarize the localization results below. For each language, list:
    - Translation quality
    - Tone feedback
    - Any corrections made

    Then, give an overall summary in 3-5 lines.

    Example output:
    === Localization Summary ===

    ☑ French:
    "Bienvenue dans notre application ! Veuillez vérifier votre adresse e-mail pour continuer."
    Quality: Excellent
    Feedback: Natural tone, no issues found.

    ☑ Spanish:
    "¡Bienvenido a nuestra aplicación! Por favor, verifica tu correo electrónico para continuar."
    Quality: Good
    Feedback: Accurate translation, tone slightly formal.

    Overall Summary:
    Both translations reviewed successfully. No major issues detected.
""";

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Agent dịch tiếng Pháp
AIAgent frenchAgent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "FrenchAgent",
        ChatOptions = new ChatOptions
        {
            Instructions = "You are a helpful assistant that translates text to French."
        }
    });

// Agent dịch tiếng Tây Ban Nha
AIAgent spanishAgent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "SpanishAgent",
        ChatOptions = new ChatOptions
        {
            Instructions = "You are a helpful assistant that translates text to Spanish."
        }
    });

// Agent kiểm định chất lượng (review)
AIAgent qualityReviewerAgent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "QualityReviewerAgent",
        ChatOptions = new ChatOptions
        {
            Instructions = qualityReviewerAgentInstructions
        }
    });

// Agent tóm tắt 
AIAgent summaryAgent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "SummaryAgent",
        ChatOptions = new ChatOptions
        {
            Instructions = summaryAgentInstructions
        }
    });

// Function dịch thuật (kết nối nhiều AI Agent)
[Description("Translate English text to French and Spanish, review, and summarize.")]
async Task<string> RunLocalizationWorkflow([Description("The English text to translate.")] string text)
{
    // Kết nối các Agent lại với nhau
    var workflow = AgentWorkflowBuilder.BuildSequential(frenchAgent, spanishAgent, qualityReviewerAgent, summaryAgent);

    AIAgent workflowAgent = workflow.AsAIAgent(
        id: "localization-workflow",
        name: "Localization Workflow",
        description: "French -> Spanish -> QA -> Summary");

    // Gọi workflow dịch thuật hiện tại bằng việc liên kết các Agent
    var response = await workflowAgent.RunAsync(text);

    // Lấy tin nhắn tóm tắt cuối cùng từ SummaryAgent
    return response.Messages.LastOrDefault()?.Text ?? "Translation failed.";
}

// Function lấy nhiệt độ
[Description("Get Current Temperature of Weather")]
async Task<float?> GetTemperature([Description("The temperature of location.")] string location)
{
    await Task.Delay(500);
    return new Random().Next(-20, 40);
}

// Agent Điều phối (Router)
AIAgent coordinatorAgent = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "CoordinatorAgent",
        ChatOptions = new ChatOptions
        {
            // Prompt định hướng cho AI
            Instructions = """
                You are a helpful and intelligent routing assistant.
                You have access to several specialized tools. 
                Analyze the user's request and automatically call the MOST appropriate tool based on its description.
                If no tool is suitable, politely inform the user.
            """,

            // Cho phép Agent sử dụng
            Tools = new List<AITool>
            {
                AIFunctionFactory.Create(GetTemperature),
                AIFunctionFactory.Create(RunLocalizationWorkflow)
            }
        }
    });

// Khởi tạo danh sách lịch sử trò chuyện
List<ChatMessage> conversationHistory = new List<ChatMessage>();

Console.WriteLine("=== AI Assistant Started (Type 'exit' to quit) ===");

while (true)
{
    Console.Write("\nYou: ");
    string userInput = Console.ReadLine() ?? string.Empty;

    if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    // Thêm câu hỏi của user vào lịch sử
    conversationHistory.Add(new ChatMessage(ChatRole.User, userInput));

    // Truyền toàn bộ lịch sử vào Coordinator Agent thay vì chỉ 1 câu text
    var response = await coordinatorAgent.RunAsync(conversationHistory);
    Console.WriteLine();

    // Lưu lại các tin nhắn (bao gồm cả quá trình gọi Tool và câu trả lời cuối cùng) vào lịch sử
    conversationHistory.AddRange(response.Messages);

    // Chỉ in ra màn hình câu trả lời cuối cùng của Assistant để tránh rối mắt
    var finalMessage = response.Messages.LastOrDefault(m => m.Role == ChatRole.Assistant);

    if (finalMessage != null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{finalMessage.AuthorName ?? "Assistant"}:");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(finalMessage.Text);
    }

    Console.ResetColor();
}