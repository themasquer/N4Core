#nullable disable

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using N4Core.Culture;
using N4Core.Culture.Utils.Bases;
using N4Core.Mappers.Utils.Bases;
using N4Core.Records.Bases;
using N4Core.Repositories.Bases;
using N4Core.Responses.Bases;
using N4Core.Responses.Managers;
using N4Core.Responses.Messages;
using N4Core.Services.Models;
using N4Core.Session.Utils.Bases;
using N4Core.Types.Extensions;
using System.Linq.Expressions;

namespace N4Core.Services.Bases
{
    public abstract class CrudServiceBase<TEntity, TQueryModel, TCommandModel> : ResponseManager, ICrudService<TQueryModel, TCommandModel>
        where TEntity : Record, new() where TQueryModel : Record, new() where TCommandModel : Record, new()
    {
        protected readonly UnitOfWorkBase _unitOfWork;
        protected readonly RepoBase<TEntity> _repo;
        protected readonly CultureUtilBase _cultureUtil;
        protected readonly SessionUtilBase _sessionUtil;
        protected readonly MapperUtilBase<TEntity, TQueryModel, TCommandModel> _mapperUtil;

        protected string _pageSessionKey;
        protected bool _usePageSession;
        protected bool _noEntityTracking;
        protected string[] _recordsPerPageCounts;

        public Languages Language { get; protected set; }
        public CrudMessagesModel Messages { get; protected set; }

        protected CrudServiceBase(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo, CultureUtilBase cultureUtil, SessionUtilBase sessionUtil, MapperUtilBase<TEntity, TQueryModel, TCommandModel> mapperUtil)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _cultureUtil = cultureUtil;
            _sessionUtil = sessionUtil;
            _mapperUtil = mapperUtil;
            _pageSessionKey = "PageSessionKey";
            _usePageSession = true;
            _noEntityTracking = true;
            Language = _cultureUtil.GetLanguage();
            _recordsPerPageCounts = ["5", "10", "25", "50", "100", Language == Languages.Türkçe ? "Tümü" : "All"];
            Messages = new CrudMessagesModel(Language);
        }

        public void Set(Languages? language = Languages.English, bool usePageSession = true, bool noEntityTracking = true, string[] recordsPerPageCounts = null, params Profile[] mapperProfiles)
        {
            Language = language.HasValue ? language.Value : _cultureUtil.GetLanguage();
            _usePageSession = usePageSession;
            _noEntityTracking = noEntityTracking;
            _mapperUtil.Set(mapperProfiles);
            _recordsPerPageCounts = recordsPerPageCounts is null ? ["5", "10", "25", "50", "100", Language == Languages.Türkçe ? "Tümü" : "All"] : recordsPerPageCounts;
            Messages = new CrudMessagesModel(Language);
        }

        public virtual IQueryable<TQueryModel> Query(IQueryable<TEntity> entityQuery)
        {
            return entityQuery.ProjectTo<TQueryModel>(_mapperUtil.Configuration);
        }

        public virtual IQueryable<TQueryModel> Query()
        {
            return Query(_repo.Query(_noEntityTracking));
        }

        public virtual IQueryable<TQueryModel> Query(Expression<Func<TQueryModel, bool>> predicate)
        {
            return Query().Where(predicate);
        }

        public virtual IQueryable<TQueryModel> Query(Expression<Func<TQueryModel, bool>> predicate, PageOrderModel pageModel)
        {
            var query = Query(predicate);
            return Paginate(query, pageModel);
        }

        public virtual IQueryable<TQueryModel> Query(PageOrderModel pageModel)
        {
            var query = Query();
            return Paginate(query, pageModel);
        }

        public virtual async Task<List<TQueryModel>> GetList(CancellationToken cancellationToken = default)
        {
            return await Query().ToListAsync(cancellationToken);
        }

        public virtual async Task<List<TQueryModel>> GetList(Expression<Func<TQueryModel, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query(predicate).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<TQueryModel>> GetList(Expression<Func<TQueryModel, bool>> predicate, PageOrderModel pageModel, CancellationToken cancellationToken = default)
        {
            return await Query(predicate, pageModel).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<TQueryModel>> GetList(PageOrderModel pageModel, CancellationToken cancellationToken = default)
        {
            return await Query(pageModel).ToListAsync(cancellationToken);
        }

        public virtual async Task<TQueryModel> GetItem(int id, CancellationToken cancellationToken = default)
        {
            return await Query().SingleOrDefaultAsync(q => q.Id == id, cancellationToken);
        }

        public virtual async Task<Response> ItemExists(Expression<Func<TQueryModel, bool>> predicate, CancellationToken cancellationToken = default)
        {
            bool exists = await Query().AnyAsync(predicate, cancellationToken);
            return exists ? Success(Messages.RecordFound) : Error(Messages.RecordNotFound);
        }

        public virtual async Task<int> GetMaxId(CancellationToken cancellationToken = default)
        {
            return await Query().MaxAsync(q => q.Id, cancellationToken);
        }

        public virtual IQueryable<TCommandModel> QueryCommand()
        {
            return _repo.Query().ProjectTo<TCommandModel>(_mapperUtil.Configuration);
        }

        public virtual async Task<TCommandModel> GetItemCommand(int id, CancellationToken cancellationToken = default)
        {
            return await QueryCommand().SingleOrDefaultAsync(q => q.Id == id, cancellationToken);
        }

        public virtual async Task<Response> Create(TCommandModel commandModel, CancellationToken cancellationToken = default)
        {
            var entity = _mapperUtil.Map(commandModel);
            _repo.Create(entity);
            await _unitOfWork.SaveAsync(cancellationToken);
            commandModel.Id = entity.Id;
            return Success(Messages.CreatedSuccessfully, commandModel.Id);
        }

        public virtual async Task<Response> Update(TCommandModel commandModel, CancellationToken cancellationToken = default)
        {
            var entity = await _repo.Query().SingleOrDefaultAsync(q => q.Id == commandModel.Id, cancellationToken);
            _repo.Update(_mapperUtil.Map(commandModel, entity));
            try
            {
                await _unitOfWork.SaveAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Error(Messages.RecordNotFound);
            }
            return Success(Messages.UpdatedSuccessfully, commandModel.Id);
        }

        public virtual async Task<Response> Delete(int id, CancellationToken cancellationToken = default)
        {
            _repo.Delete(e => e.Id == id);
            await _unitOfWork.SaveAsync(cancellationToken);
            return Success(Messages.DeletedSuccessfully);
        }

        public virtual async Task<Response> Delete(CancellationToken cancellationToken = default)
        {
            _repo.Delete();
            await _unitOfWork.SaveAsync(cancellationToken);
            return Success(Messages.DeletedSuccessfully);
        }

        public virtual IQueryable<TQueryModel> Paginate(IQueryable<TQueryModel> query, PageOrderModel pageModel)
        {
            pageModel.Language = Language;
            pageModel.RecordsPerPageCounts = _recordsPerPageCounts.ToList();
            if (_usePageSession && pageModel.PageSession)
            {
                var pageSessionModel = _sessionUtil.Get<PageOrderModel>(_pageSessionKey);
                if (pageSessionModel is not null)
                {
                    pageModel.PageNumber = pageSessionModel.PageNumber;
                    pageModel.RecordsPerPageCount = pageSessionModel.RecordsPerPageCount;
                    pageModel.RecordsPerPageCounts = pageSessionModel.RecordsPerPageCounts;
                    pageModel.OrderExpression = pageSessionModel.OrderExpression;
                    pageModel.OrderExpressions = pageSessionModel.OrderExpressions;
                }
            }
            query = query.Paginate(pageModel);
            if (_usePageSession)
                _sessionUtil.Set(pageModel, _pageSessionKey);
            return query;
        }

        public virtual List<TQueryModel> Paginate(List<TQueryModel> list, PageOrderModel pageModel)
        {
            return Paginate(list.AsQueryable(), pageModel).ToList();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
            _repo.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public abstract class CrudServiceBase<TEntity, TModel> : CrudServiceBase<TEntity, TModel, TModel>, ICrudService<TModel, TModel> where TEntity : Record, new() where TModel : Record, new()
    {
        protected CrudServiceBase(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo, CultureUtilBase cultureUtil, SessionUtilBase sessionUtil,
            MapperUtilBase<TEntity, TModel, TModel> mapperUtil) : base(unitOfWork, repo, cultureUtil, sessionUtil, mapperUtil)
        {
        }
    }
}
