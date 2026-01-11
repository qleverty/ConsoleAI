using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
		Console.OutputEncoding = System.Text.Encoding.UTF8;
		
        if (args.Length < 2)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please provide both model and query.");
            Console.ResetColor();
            return;
        }

        string model = args[0].Trim();
        string userQuery = string.Join(" ", args.Skip(1)).Trim();

        if (string.IsNullOrWhiteSpace(model))
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Model cannot be empty.");
            Console.ResetColor();
            return;
        }

        if (string.IsNullOrWhiteSpace(userQuery))
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Query cannot be empty.");
            Console.ResetColor();
            return;
        }

        string baseDir = AppContext.BaseDirectory;
        string keyPath = Path.Combine(baseDir, "openrouterkey.txt");
		string promptPath = Path.Combine(baseDir, "prompt.txt");
		
		string apiKey = "";
		if (!File.Exists(keyPath) || string.IsNullOrWhiteSpace(apiKey = File.ReadAllText(keyPath).Trim()))
		{
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("API key file not found.");
			Console.WriteLine($"Go to https://openrouter.ai/settings/keys to get an API key and write it in {keyPath}");
			Console.ResetColor();
			return;
		}
		
		string systemPrompt;
        if (File.Exists(promptPath))
        {
            try
            {
                systemPrompt = File.ReadAllText(promptPath).Trim();
            }
            catch (Exception)
            {
                systemPrompt = "";
            }
        }
        else
        {
            systemPrompt = "";
        }

        ConsoleColor assistantColor = model.ToLower().Contains("gemini") ? ConsoleColor.Magenta :
                                     model.ToLower().Contains("deepseek") ? ConsoleColor.Blue :
                                     model.ToLower().Contains("llama") ? ConsoleColor.Yellow :
                                     model.ToLower().Contains("gpt") ? ConsoleColor.Green :
                                     ConsoleColor.White;

        List<Message> conversation = new List<Message>();
        if (!string.IsNullOrWhiteSpace(systemPrompt))
        {
            conversation.Add(new Message { role = "system", content = systemPrompt });
        }
        conversation.Add(new Message { role = "user", content = userQuery });

        string? assistantResponse = SendRequest(model, apiKey, conversation);
        if (assistantResponse == null)
        {
            return;
        }
        Console.WriteLine();
        Console.ForegroundColor = assistantColor;
        Console.WriteLine(assistantResponse);
        Console.ResetColor();

        while (true)
        {
            Console.WriteLine();
            Console.Write(">>> ");
            string? nextInput = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(nextInput))
            {
                return;
            }
            conversation.Add(new Message { role = "user", content = nextInput });
            assistantResponse = SendRequest(model, apiKey, conversation);
            if (assistantResponse == null)
            {
                return;
            }
            Console.ForegroundColor = assistantColor;
            Console.WriteLine();
            Console.WriteLine(assistantResponse);
            Console.ResetColor();
        }
    }

    static string? SendRequest(string model, string apiKey, List<Message> conversation)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
        var requestBody = new RequestBody
        {
            model = model,
            messages = conversation.ToArray()
        };
        string jsonBody = JsonSerializer.Serialize(requestBody, SourceGenerationContext.Default.RequestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        try
        {
            var response = client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content).Result;
            if (response.IsSuccessStatusCode)
            {
                string messageContent = (JsonNode.Parse(response.Content.ReadAsStringAsync().Result)?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? "").Trim();
                conversation.Add(new Message { role = "assistant", content = messageContent });
                return messageContent;
            }
            else
            {
                string errorMessage;
                try
                {
                    errorMessage = JsonNode.Parse(response.Content.ReadAsStringAsync().Result)?["error"]?["message"]?.GetValue<string>() ?? "Unknown error";
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(errorMessage.ToLower().Contains("limit exceeded") 
                        ? "Free requests limit per day is exceeded" 
                        : errorMessage);
                    Console.ResetColor();
                }
                catch
                {
                    errorMessage = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("An error occurred: " + errorMessage);
                    Console.ResetColor();
                }
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("An error occurred: " + ex.Message);
            Console.ResetColor();
            return null;
        }
    }
}

[JsonSerializable(typeof(RequestBody))]
[JsonSerializable(typeof(Message))]
internal partial class SourceGenerationContext : JsonSerializerContext { }

class Message
{
    public string role { get; set; } = "";
    public string content { get; set; } = "";
}

class RequestBody
{
    public string model { get; set; } = "";
    public Message[] messages { get; set; } = Array.Empty<Message>();
}