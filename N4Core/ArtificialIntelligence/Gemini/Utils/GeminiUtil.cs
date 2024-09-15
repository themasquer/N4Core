using DotnetGeminiSDK.Client.Interfaces;
using N4Core.ArtificialIntelligence.Gemini.Utils.Bases;

namespace N4Core.ArtificialIntelligence.Gemini.Utils
{
    public class GeminiUtil : GeminiUtilBase
    {
        public GeminiUtil(IGeminiClient geminiClient) : base(geminiClient)
        {
        }
    }
}
