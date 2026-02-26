
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credential = new ApiKeyCredential(config["GitHubModels:Token"] ?? throw new InvalidOperationException("Missing configuration: GitHubModels:Token."));
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.github.ai/inference")
};

IChatClient chatClient = new OpenAIClient(credential, options)
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

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
string qualityReviewerAgentInstructions = """
    You are a multilingual translation quality reviewer.
    Check the translations for grammar accuracy, tone consistency, and cultural fit
    compared to the original English text.

    Give a brief summary with a quality rating (Excellent / Good / Needs Review).

    Example out:
    Quality: Excellent
    Feedback: Accurate translation, friendly tone preserved, minor punctuation tweaks only.
    """;

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

// Kết nối các Agent lại với nhau
var workflow = AgentWorkflowBuilder.BuildSequential(
    frenchAgent, spanishAgent, qualityReviewerAgent, summaryAgent);

AIAgent workflowAgent = workflow.AsAIAgent(
    id: "localization-workflow",
    name: "Localization Workflow",
    description: "French -> Spanish -> QA -> Summary");

// Truyền input từ người dùng vào Agent
Console.Write("\nYou: Input text English: ");
string userInput = Console.ReadLine() ?? string.Empty;

var reponse = await workflowAgent.RunAsync(userInput);
Console.WriteLine();

foreach(var message in reponse.Messages)
{
    // Tên là màu vàng
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"{message.AuthorName}:");

    // Bảng dịch, tóm tắt sẽ hiện xanh lá
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(message.Text);

    Console.WriteLine();
}


