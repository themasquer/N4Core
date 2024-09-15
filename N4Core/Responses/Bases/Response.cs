using N4Core.Records.Bases;

namespace N4Core.Responses.Bases
{
    public class Response : Record
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        public Response(bool isSuccessful, string message, int id) : base(id)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }

    public class Response<TResponseType> : IResponseData<TResponseType>
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public TResponseType Data { get; }

        public Response(bool isSuccessful, string message, TResponseType data)
        {
            IsSuccessful = isSuccessful;
            Message = message;
            Data = data;
        }
    }
}
