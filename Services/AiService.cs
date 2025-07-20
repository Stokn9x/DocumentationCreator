using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetEnv;

namespace DocumentationCreator.Services
{
    public class AiService
    {
        private readonly AzureOpenAIClient _azureOpenAIClient;
        private readonly ChatClient _chatClient;
        private readonly string _deploymentName;

        public AiService()
        {
            Env.Load();

            var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            var key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");
            _deploymentName = "gpt-4o";

            Console.WriteLine($"Using Azure OpenAI endpoint: {endpoint}");
            Console.WriteLine($"Using deployment name: {_deploymentName}");

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Missing Azure OpenAI credentials");

            _azureOpenAIClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
            _chatClient = _azureOpenAIClient.GetChatClient(_deploymentName);
        }

        public async Task<string> AnalyzeCodeChunkAsync(string codeChunk, string category)
        {
            var prompt = $$"""
                Du er en professionel softwarearkitekt. Du får her et uddrag af C#-kode i kategorien "{{category}}".

                Din opgave er at:
                - Forklare hvad filen overordnet gør (formål).
                - Fremhæv vigtige begreber, mønstre eller anvendte teknologier.
                - Undlad at skrive kodeeksempler eller prøve at gætte på klasser/metoder du ikke ser direkte.
                - Undlad at genopfinde implementeringer — brug kun den kode du ser.
                - Returnér ren Markdown uden introduktion eller afslutning.

                Her er koden:
                {{codeChunk}}
                """;

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("Du skriver kun Markdown-dokumentation. Ingen chat, intet smalltalk."),
                new UserChatMessage(prompt)
            };

            var response = await _chatClient.CompleteChatAsync(messages);
            return response.Value.Content.Last().Text.Trim();
        }
    }
}
