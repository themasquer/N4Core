namespace N4Core.ArtificialIntelligence.Models
{
    public class AiViewModel
    {
        public AiRequestModel Request { get; set; }
        public AiResponseModel Response { get; set; }

        public AiViewModel(AiRequestModel request, AiResponseModel response)
        {
            Request = request;
            Response = response;
        }

        public AiViewModel()
        {
            Request = new AiRequestModel();
            Response = new AiResponseModel();
        }
    }
}
