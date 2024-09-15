using N4Core.Culture.Utils.Bases;
using N4Core.Files.Utils.Bases;
using N4Core.Mappers.Utils.Bases;
using N4Core.Records.Bases;
using N4Core.Reflection.Utils.Bases;
using N4Core.Reports.Utils.Bases;
using N4Core.Repositories.Bases;
using N4Core.Services.Bases;
using N4Core.Session.Utils.Bases;

namespace N4Core.Services
{
    public class Service<TEntity, TQueryModel, TCommandModel> : ServiceBase<TEntity, TQueryModel, TCommandModel>
        where TEntity : Record, new() where TQueryModel : Record, new() where TCommandModel : Record, new()
    {
        public Service(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo, ReflectionUtilBase reflectionUtil, CultureUtilBase cultureUtil, SessionUtilBase sessionUtil,
            MapperUtilBase<TEntity, TQueryModel, TCommandModel> mapperUtil, FileUtilBase fileUtil, ReportUtilBase reportUtil) 
            : base(unitOfWork, repo, reflectionUtil, cultureUtil, sessionUtil, mapperUtil, fileUtil, reportUtil)
        {
        }
    }

    public class Service<TEntity, TModel> : ServiceBase<TEntity, TModel> where TEntity : Record, new() where TModel : Record, new()
    {
        public Service(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo, ReflectionUtilBase reflectionUtil, CultureUtilBase cultureUtil, SessionUtilBase sessionUtil, 
            MapperUtilBase<TEntity, TModel, TModel> mapperUtil, FileUtilBase fileUtil, ReportUtilBase reportUtil) : base(unitOfWork, repo, reflectionUtil, cultureUtil, sessionUtil, mapperUtil, fileUtil, reportUtil)
        {
        }
    }
}
