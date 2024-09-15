using MediatR;
using N4Core.Handlers.Bases;
using N4Core.Records.Bases;
using N4Core.Repositories.Bases;
using N4Core.Requests.Bases;
using N4Core.Responses.Bases;

namespace N4Core.Handlers
{
    public class ApiHandler<TEntity, TRequest, TResponse> : ApiHandlerBase<TEntity, TRequest, TResponse>
        where TEntity : Record, new() where TRequest : Request, IRequest<Response<IQueryable<TResponse>>>, new() where TResponse : Record, new()
    {
        public ApiHandler(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo) : base(unitOfWork, repo)
        {
        }
    }
}
