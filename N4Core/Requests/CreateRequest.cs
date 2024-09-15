using MediatR;
using N4Core.Records.Bases;
using N4Core.Requests.Bases;
using N4Core.Requests.Enums;
using N4Core.Responses.Bases;

namespace N4Core.Requests
{
    public class CreateRequest : Request, IRequest<Response<IQueryable<Record>>>
    {
        public CreateRequest() : base(RequestOperations.Create)
        {
        }
    }
}
