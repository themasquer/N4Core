#nullable disable

using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using N4Core.Culture;
using N4Core.Mappers.Utils;
using N4Core.Records.Bases;
using N4Core.Repositories.Bases;
using N4Core.Requests.Bases;
using N4Core.Requests.Enums;
using N4Core.Responses.Bases;
using N4Core.Responses.Managers;
using N4Core.Responses.Messages;

namespace N4Core.Handlers.Bases
{
    public abstract class ApiHandlerBase<TEntity, TRequest, TResponse> : ResponseManager, IRequestHandler<TRequest, Response<IQueryable<TResponse>>>
        where TEntity : Record, new() where TRequest : Request, IRequest<Response<IQueryable<TResponse>>>, new() where TResponse : Record, new()
    {
        protected readonly UnitOfWorkBase _unitOfWork;
        protected readonly RepoBase<TEntity> _repo;

        protected readonly MapperUtil<TEntity, TResponse, TRequest> _mapperUtil;

        public virtual bool NoEntityTracking { get; }
        public CrudMessagesModel Messages { get; protected set; }

        protected ApiHandlerBase(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _mapperUtil = new MapperUtil<TEntity, TResponse, TRequest>();
            Messages = new CrudMessagesModel(Languages.English);
        }

        public virtual async Task<Response<IQueryable<TResponse>>> Handle(TRequest request, CancellationToken cancellationToken)
        {
            if (request.Operation == RequestOperations.None)
                return Error<IQueryable<TResponse>>(Messages.RequestMethodNotConfigured);
            string message = string.Empty;
            IQueryable<TResponse> query = null;
            TEntity entity;
            switch (request.Operation)
            {
                case RequestOperations.Query:
                    _mapperUtil.Set(request.MapperProfiles);
                    query = await Task.FromResult(_repo.Query(NoEntityTracking).ProjectTo<TResponse>(_mapperUtil.Configuration));
                    break;
                case RequestOperations.Create:
                    _mapperUtil.Set(request.MapperProfiles);
                    entity = _mapperUtil.Map(request);
                    _repo.Create(entity);
                    await _unitOfWork.SaveAsync(cancellationToken);
                    message = Messages.CreatedSuccessfully;
                    break;
                case RequestOperations.Update:
                    _mapperUtil.Set(request.MapperProfiles);
                    entity = await _repo.Query().SingleOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
                    _repo.Update(_mapperUtil.Map(request, entity));
                    try
                    {
                        await _unitOfWork.SaveAsync(cancellationToken);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Error<IQueryable<TResponse>>(Messages.RecordNotFound);
                    }
                    message = Messages.UpdatedSuccessfully;
                    break;
                case RequestOperations.Delete:
                    _repo.Delete(r => r.Id == request.Id);
                    await _unitOfWork.SaveAsync(cancellationToken);
                    message = Messages.DeletedSuccessfully;
                    break;
            }
            return Success(message, query);
        }
    }

    public abstract class ApiHandler<TEntity, TRequest> : ApiHandlerBase<TEntity, TRequest, Record>
        where TEntity : Record, new() where TRequest : Request, IRequest<Response<IQueryable<Record>>>, new()
    {
        protected ApiHandler(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo) : base(unitOfWork, repo)
        {
        }
    }
}
