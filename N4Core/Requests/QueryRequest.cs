using MediatR;
using N4Core.Records.Bases;
using N4Core.Requests.Bases;
using N4Core.Requests.Enums;
using N4Core.Responses.Bases;

namespace N4Core.Requests
{
    public class QueryRequest<TResponse> : Request, IRequest<Response<IQueryable<TResponse>>> where TResponse : Record, new()
    {
        public QueryRequest() : base(RequestOperations.Query)
        {
        }
    }
}
