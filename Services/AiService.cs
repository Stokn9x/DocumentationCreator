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
using System.Text.RegularExpressions;

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

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Missing Azure OpenAI credentials");

            _azureOpenAIClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
            _chatClient = _azureOpenAIClient.GetChatClient(_deploymentName);
        }

        public async Task<string> AnalyzeCodeChunkAsync(string codeChunk, string category)
        {
            var prompt = $$"""
                Du er en professionel softwarearkitekt. Du får her et uddrag af C#-kode i kategorien "{{category}}".

                Din opgave er at skrive **klar og professionel dokumentation** i Markdown. Fokusér på formål og nøgleelementer – **ikke overflødige detaljer eller kode du ikke ser**.

                Retningslinjer:
                - Start med en kort opsummering af filens formål.
                - Forklar centrale begreber, mønstre eller teknologier.
                - Brug punktopstillinger **kun når det giver mening** – ellers brug almindelig tekst.
                - Undlad at vise kodeeksempler, eller forsøge at gætte på manglende elementer.
                - Undgå gentagelser og hold strukturen let at scanne visuelt.
                - Returnér **kun Markdown-indhold** – ingen chat eller forklaringer.

                Her er koden:
                {{codeChunk}}
                """;

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("Du skriver kun Markdown-dokumentation. Ingen chat, intet smalltalk."),
                new UserChatMessage(prompt)
            };

            var response = await _chatClient.CompleteChatAsync(messages);

            var raw = response.Value.Content.Last().Text.Trim();
            var cleaned = Regex.Replace(raw, @"```markdown\s*", "");
            cleaned = Regex.Replace(cleaned, @"\s*```", "");

            return cleaned;

        }
    }
}
