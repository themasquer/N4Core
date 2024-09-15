#nullable disable

using N4Core.Responses.Bases;

namespace N4Core.Responses
{
    public class ErrorResponse : Response
    {
        public ErrorResponse(string message) : base(false, message, default)
        {
        }

        public ErrorResponse() : base(false, string.Empty, default)
        {
        }
    }

    public class ErrorResponse<TResponseType> : Response<TResponseType>
    {
        public ErrorResponse(string message, TResponseType data) : base(false, message, data)
        {
        }

        public ErrorResponse(string message) : base(false, message, default)
        {
        }

        public ErrorResponse(TResponseType data) : base(false, string.Empty, data)
        {
        }

        public ErrorResponse() : base(false, string.Empty, default)
        {
        }
    }
}
