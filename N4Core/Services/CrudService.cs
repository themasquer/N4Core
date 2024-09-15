using N4Core.Culture.Utils.Bases;
using N4Core.Mappers.Utils.Bases;
using N4Core.Records.Bases;
using N4Core.Repositories.Bases;
using N4Core.Services.Bases;
using N4Core.Session.Utils.Bases;

namespace N4Core.Services
{
    public class CrudService<TEntity, TQueryModel, TCommandModel> : CrudServiceBase<TEntity, TQueryModel, TCommandModel> 
        where TEntity : Record, new() where TQueryModel : Record, new() where TCommandModel : Record, new()
    {
        public CrudService(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo, CultureUtilBase cultureUtil, SessionUtilBase sessionUtil,
            MapperUtilBase<TEntity, TQueryModel, TCommandModel> mapperUtil) : base(unitOfWork, repo, cultureUtil, sessionUtil, mapperUtil)
        {
        }
    }

    public class CrudService<TEntity, TModel> : CrudServiceBase<TEntity, TModel> where TEntity : Record, new() where TModel : Record, new()
    {
        public CrudService(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo, CultureUtilBase cultureUtil, SessionUtilBase sessionUtil, 
            MapperUtilBase<TEntity, TModel, TModel> mapperUtil) : base(unitOfWork, repo, cultureUtil, sessionUtil, mapperUtil)
        {
        }
    }
}
