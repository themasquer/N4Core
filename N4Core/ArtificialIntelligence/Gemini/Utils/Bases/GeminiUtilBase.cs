#nullable disable

using DotnetGeminiSDK.Client.Interfaces;
using N4Core.ArtificialIntelligence.Messages;
using N4Core.ArtificialIntelligence.Models;
using N4Core.ArtificialIntelligence.Utils.Bases;
using N4Core.Culture;
using N4Core.Types.Extensions;

namespace N4Core.ArtificialIntelligence.Gemini.Utils.Bases
{
    public abstract class GeminiUtilBase : IAiUtil
    {
        protected readonly IGeminiClient _geminiClient;

        public AiMessagesModel Messages { get; protected set; }

        protected GeminiUtilBase(IGeminiClient geminiClient)
        {
            _geminiClient = geminiClient;
            Messages = new AiMessagesModel(Languages.English);
        }

        public void Set(Languages language)
        {
            Messages = new AiMessagesModel(language);
        }

        public virtual async Task<AiResponseModel> Prompt(AiRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                return new AiResponseModel(Messages.EmptyRequestText);
            var response = await _geminiClient.TextPrompt(request.Text);
            if (response is null)
                return new AiResponseModel(Messages.NoResponse);
            return new AiResponseModel(response.Candidates.FirstOrDefault()?.Content.Parts.FirstOrDefault()?.Text?.ReplaceNewLineWithLineBreak() ?? Messages.EmptyResponse);
        }
    }
}
