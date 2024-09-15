#nullable disable

using AutoMapper;
using N4Core.Culture;
using N4Core.Responses.Bases;
using N4Core.Services.Models;
using System.Linq.Expressions;

namespace N4Core.Services.Bases
{
    public interface ICrudService<TQueryModel, TCommandModel> : IDisposable
    {
        public void Set(Languages? language = Languages.English, bool usePageSession = true, bool noEntityTracking = true, string[] recordsPerPageCounts = null, params Profile[] mapperProfiles);
        public IQueryable<TQueryModel> Query();
        public IQueryable<TQueryModel> Query(Expression<Func<TQueryModel, bool>> predicate);
        public IQueryable<TQueryModel> Query(Expression<Func<TQueryModel, bool>> predicate, PageOrderModel pageModel);
        public IQueryable<TQueryModel> Query(PageOrderModel pageModel);
        public Task<List<TQueryModel>> GetList(CancellationToken cancellationToken = default);
        public Task<List<TQueryModel>> GetList(Expression<Func<TQueryModel, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<List<TQueryModel>> GetList(Expression<Func<TQueryModel, bool>> predicate, PageOrderModel pageModel, CancellationToken cancellationToken = default);
        public Task<List<TQueryModel>> GetList(PageOrderModel pageModel, CancellationToken cancellationToken = default);
        public Task<TQueryModel> GetItem(int id, CancellationToken cancellationToken = default);
        public Task<Response> ItemExists(Expression<Func<TQueryModel, bool>> predicate, CancellationToken cancellationToken = default);
        public Task<int> GetMaxId(CancellationToken cancellationToken = default);
        public IQueryable<TCommandModel> QueryCommand();
        public Task<TCommandModel> GetItemCommand(int id, CancellationToken cancellationToken = default);
        public Task<Response> Create(TCommandModel commandModel, CancellationToken cancellationToken = default);
        public Task<Response> Update(TCommandModel commandModel, CancellationToken cancellationToken = default);
        public Task<Response> Delete(int id, CancellationToken cancellationToken = default);
        public Task<Response> Delete(CancellationToken cancellationToken = default);
        public IQueryable<TQueryModel> Paginate(IQueryable<TQueryModel> query, PageOrderModel pageModel);
        public List<TQueryModel> Paginate(List<TQueryModel> list, PageOrderModel pageModel);
    }

    public interface ICrudService<TModel> : IDisposable
    {
        public IQueryable<TModel> Query();
        public Response Create(TModel model);
        public Response Update(TModel model);
        public Response Delete(int id);
    }
}
