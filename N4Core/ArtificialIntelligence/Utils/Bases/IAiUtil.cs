#nullable disable

using N4Core.ArtificialIntelligence.Models;
using N4Core.Culture;

namespace N4Core.ArtificialIntelligence.Utils.Bases
{
    public interface IAiUtil
    {
        public void Set(Languages language);
        public Task<AiResponseModel> Prompt(AiRequestModel request);
        public Task<AiViewModel> Prompt(AiViewModel viewModel) => Task.FromResult(new AiViewModel(viewModel.Request, Prompt(viewModel.Request).Result));
    }
}
