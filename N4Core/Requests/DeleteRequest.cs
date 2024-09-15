using MediatR;
using N4Core.Records.Bases;
using N4Core.Requests.Bases;
using N4Core.Requests.Enums;
using N4Core.Responses.Bases;

namespace N4Core.Requests
{
    public class DeleteRequest : Request, IRequest<Response<IQueryable<Record>>>
    {
        public DeleteRequest() : base(RequestOperations.Delete)
        {
        }
    }
}
