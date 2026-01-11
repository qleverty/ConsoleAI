<img width="773" height="154" alt="Image" src="https://github.com/user-attachments/assets/a9a61505-71ff-4296-9776-8923d3ddc5c9" />

# ConsoleAI

A lightweight command-line interface for interacting with multiple AI models via OpenRouter.

I got tired of constantly opening websites to ask AI something, so I decided to integrate a simple AI API directly into the terminal. The program uses OpenRouter API, which supports lots of models and doesn't require a VPN. And essentially, this is my first serious C# project.

## Installation & Setup

1. Download and run [ConsoleAI-Setup-Win64.exe](https://github.com/qleverty/ConsoleAI/releases/latest)
2. Click **Install** to complete the installation
3. Register at [OpenRouter](https://openrouter.ai/settings/keys) and create an API key
4. Open `openrouterkey.txt` in the installation folder
5. Paste your API key and save the file (use admin rights if file is locked)

## Usage

Use these commands in your terminal to chat with different AI models:

- **`dp [your question]`** - DeepSeek
- **`gn [your question]`** - Gemini 2.0 Flash
- **`gpt [your question]`** - GPT-4o Mini
- **`lm [your question]`** - Llama 3.1 8B

Example: `dp Why is the sky blue?` or `gpt Explain recursion simply`

After the first response, you can continue the conversation by typing follow-up questions. Press Enter on an empty line to exit.

## Custom System Prompt (Optional)

You can create a `prompt.txt` file in the installation folder to set a default system prompt that will be used for all conversations. This allows you to define the AI's behavior and tone.

## Customization

Model endpoints are stored in `.bat` files in the installation folder. If OpenRouter updates their model paths, you can edit these files to use the current endpoints. You can also create your own `.bat` files to add custom models:

```batch
@echo off
ConsoleAI.exe "your-model-path" %*
```

Browse available models at [OpenRouter Models](https://openrouter.ai/models).

## Notes

- If you really add custom models, check their pricing at OpenRouter - token costs vary significantly between models
- Conversation history is maintained in memory during the session but not saved between runs
- Some models have rate limits or require credits, so monitor your OpenRouter usage
- Since the program receives your query as command-line arguments, certain special characters (like `&`, `|`, `<`, `>`) may need escaping or quotes
