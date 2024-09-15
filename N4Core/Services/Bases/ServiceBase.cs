#nullable disable

using LinqKit;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using N4Core.Culture.Utils.Bases;
using N4Core.Files.Bases;
using N4Core.Files.Models;
using N4Core.Files.Models.Bases;
using N4Core.Files.Utils.Bases;
using N4Core.Mappers.Utils.Bases;
using N4Core.Records.Bases;
using N4Core.Reflection.Attributes;
using N4Core.Reflection.Models;
using N4Core.Reflection.Utils.Bases;
using N4Core.Reports.Utils.Bases;
using N4Core.Repositories.Bases;
using N4Core.Responses.Bases;
using N4Core.Responses.Messages;
using N4Core.Services.Configs;
using N4Core.Services.Models;
using N4Core.Session.Utils.Bases;
using N4Core.Types.Extensions;
using System.Linq.Expressions;

namespace N4Core.Services.Bases
{
    public abstract class ServiceBase<TEntity, TQueryModel, TCommandModel> : CrudServiceBase<TEntity, TQueryModel, TCommandModel>, IService<TQueryModel, TCommandModel>
        where TEntity : Record, new() where TQueryModel : Record, new() where TCommandModel : Record, new()
    {
        protected readonly ReflectionUtilBase _reflectionUtil;
        protected readonly FileUtilBase _fileUtil;
        protected readonly ReportUtilBase _reportUtil;

        protected readonly List<ReflectionPropertyModel> _reflectionOrderingProperties;
        protected readonly List<ReflectionPropertyModel> _reflectionFilteringProperties;

        public ServiceConfig Config { get; protected set; }
        public ViewModel ViewModel { get; protected set; }

        protected ServiceBase(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo, ReflectionUtilBase reflectionUtil, CultureUtilBase cultureUtil, SessionUtilBase sessionUtil, 
            MapperUtilBase<TEntity, TQueryModel, TCommandModel> mapperUtil, FileUtilBase fileUtil, ReportUtilBase reportUtil) : base(unitOfWork, repo, cultureUtil, sessionUtil, mapperUtil)
        {
            _reflectionUtil = reflectionUtil;
            _fileUtil = fileUtil;
            _reportUtil = reportUtil;
            _pageSessionKey = "PageOrderFilterSessionKey";
            _reflectionOrderingProperties = _reflectionUtil.GetReflectionPropertyModelProperties<TQueryModel>(TagAttributes.Order);
            _reflectionFilteringProperties = _reflectionUtil.GetReflectionPropertyModelProperties<TQueryModel>(TagAttributes.StringFilter);
            Config = new ServiceConfig(); 
            ViewModel = new ViewModel(Language);
            Messages = new MessagesModel(Language);
        }

        public void Set(Action<ServiceConfig> config)
        {
            config.Invoke(Config);
            Set(Config.Language, Config.UsePageSession, Config.NoEntityTracking, Config.RecordsPerPageCounts, Config.MapperProfiles);
            _fileUtil.Set(Config.FileLengthInMegaBytes, Config.FileExtensions, Config.Directories);
            ViewModel = new ViewModel(Language)
            {
                PageOrderFilter = Config.PageOrderFilter,
                ListCards = Config.ListCards,
                Modal = Config.Modal,
                FileOperations = Config.FileOperations,
                ExportOperation = Config.ExportOperation,
                TimePicker = Config.TimePicker
            };
            Messages = new MessagesModel(Language);
        }

        public virtual IQueryable<TQueryModel> Query(PageOrderFilterModel pageModel)
        {
            IQueryable<TQueryModel> query;
            IQueryable<TEntity> entityQuery = _repo.Query(true);
            if (_usePageSession && pageModel.PageSession)
            {
                var pageSessionModel = _sessionUtil.Get<PageOrderFilterModel>(_pageSessionKey);
                if (pageSessionModel is not null)
                {
                    pageModel.OrderDirectionDescending = pageSessionModel.OrderDirectionDescending;
                    pageModel.Filter = pageSessionModel.Filter;
                }
            }
            if (_reflectionOrderingProperties is null)
            {
                if (pageModel.OrderExpressions is not null && pageModel.OrderExpressions.Any())
                    entityQuery = entityQuery.OrderBy(pageModel);
                query = Query(entityQuery);
                ViewModel.OrderExpressions = pageModel.OrderExpressions;
            }
            else
            {
                query = Query(entityQuery);
                var orderExpressions = _reflectionOrderingProperties.Select(pm => 
                    !string.IsNullOrWhiteSpace(pm.DisplayName) ? pm.DisplayName.GetDisplayName(Language) : pm.Name.GetDisplayName(Language)).ToList();
                ViewModel.OrderExpressions = orderExpressions.Select(oe => new SelectListItem(oe, oe)).ToList();
                if (_reflectionOrderingProperties.Any() && !string.IsNullOrWhiteSpace(pageModel.OrderExpression))
                {
                    var propertyForOrdering = _reflectionOrderingProperties.FirstOrDefault(p =>
                        p.DisplayName.GetDisplayName(Language) == pageModel.OrderExpression);
                    if (propertyForOrdering is null)
                        propertyForOrdering = _reflectionOrderingProperties.FirstOrDefault(p => p.Name == pageModel.OrderExpression);
                    if (propertyForOrdering is not null)
                    {
                        query = pageModel.OrderDirectionDescending ? query.OrderByDescending(_reflectionUtil.GetExpression<TQueryModel>(propertyForOrdering.Name)) :
                            query.OrderBy(_reflectionUtil.GetExpression<TQueryModel>(propertyForOrdering.Name));
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(pageModel.Filter))
            {
                if (_reflectionFilteringProperties is not null && _reflectionFilteringProperties.Any())
                {
                    var predicate = _reflectionUtil.GetPredicateContainsExpression<TQueryModel>(_reflectionFilteringProperties[0].Name, pageModel.Filter);
                    for (var i = 1; i < _reflectionFilteringProperties.Count; i++)
                    {
                        predicate = predicate.Or(_reflectionUtil.GetPredicateContainsExpression<TQueryModel>(_reflectionFilteringProperties[i].Name, pageModel.Filter));
                    }
                    query = query.Where(predicate);
                }
            }
            query = Paginate(query, pageModel);
            ViewModel.RecordsPerPageCounts = pageModel.RecordsPerPageCounts;
            ViewModel.PageNumber = pageModel.PageNumber;
            ViewModel.RecordsPerPageCount = pageModel.RecordsPerPageCount;
            ViewModel.OrderExpression = pageModel.OrderExpression;
            ViewModel.OrderDirectionDescending = pageModel.OrderDirectionDescending;
            ViewModel.Filter = pageModel.Filter;
            ViewModel.TotalRecordsCount = pageModel.TotalRecordsCount;
            ViewModel.Message = ViewModel.TotalRecordsCount == 0 ? Messages.RecordNotFound : ViewModel.TotalRecordsCount == 1 ?
                ViewModel.TotalRecordsCount + " " + Messages.RecordFound : ViewModel.TotalRecordsCount + " " + Messages.RecordsFound;
            return query;
        }

        public virtual async Task<List<TQueryModel>> GetList(PageOrderFilterModel pageModel, CancellationToken cancellationToken = default)
        {
            var query = Query(pageModel);
            var list = await query.ToListAsync(cancellationToken);
            if (Config.FileOperations)
                _fileUtil.UpdateImgSrc(list);
            return list;
        }

        public override async Task<List<TQueryModel>> GetList(CancellationToken cancellationToken = default)
        {
            var list = await base.GetList(cancellationToken);
            ViewModel.TotalRecordsCount = list.Count;
            ViewModel.Message = ViewModel.TotalRecordsCount == 0 ? Messages.RecordNotFound : ViewModel.TotalRecordsCount == 1 ?
                ViewModel.TotalRecordsCount + " " + Messages.RecordFound : ViewModel.TotalRecordsCount + " " + Messages.RecordsFound;
            if (Config.FileOperations)
                _fileUtil.UpdateImgSrc(list);
            return list;
        }

        public override async Task<List<TQueryModel>> GetList(Expression<Func<TQueryModel, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var list = await base.GetList(predicate, cancellationToken);
            ViewModel.TotalRecordsCount = list.Count;
            ViewModel.Message = ViewModel.TotalRecordsCount == 0 ? Messages.RecordNotFound : ViewModel.TotalRecordsCount == 1 ?
                ViewModel.TotalRecordsCount + " " + Messages.RecordFound : ViewModel.TotalRecordsCount + " " + Messages.RecordsFound;
            if (Config.FileOperations)
                _fileUtil.UpdateImgSrc(list);
            return list;
        }

        public override async Task<TQueryModel> GetItem(int id, CancellationToken cancellationToken = default)
        {
            var item = await base.GetItem(id, cancellationToken);
            if (Config.FileOperations)
                _fileUtil.UpdateImgSrc(item);
            return item;
        }

        public override async Task<TCommandModel> GetItemCommand(int id, CancellationToken cancellationToken = default)
        {
            var item = await base.GetItemCommand(id, cancellationToken);
            if (Config.FileOperations)
                _fileUtil.UpdateImgSrc(item);
            return item;
        }

        public override async Task<Response> Create(TCommandModel commandModel, CancellationToken cancellationToken = default)
        {
            RecordFileModel fileModel = null;
            RecordFile file = null;
            var entity = _mapperUtil.Map(commandModel);
            if (Config.FileOperations && _repo.ReflectionRecordModel.HasFile && commandModel is RecordFileModel)
            {
                fileModel = commandModel as RecordFileModel;
                if (_fileUtil.CheckFile(fileModel.FormFile) == false)
                    return Error((Messages as MessagesModel).InvalidFileExtensionOrFileLength);
                file = entity as RecordFile;
                _fileUtil.UpdateFile(fileModel.FormFile, file);
            }
            _repo.Create(entity);
            await _unitOfWork.SaveAsync(cancellationToken);
            commandModel.Id = entity.Id;
            if (fileModel is not null && file is not null)
            {
                _fileUtil.UploadFile(fileModel.FormFile, file);
            }
            return Success(Messages.CreatedSuccessfully, commandModel.Id);
        }

        public override async Task<Response> Update(TCommandModel commandModel, CancellationToken cancellationToken = default)
        {
            RecordFileModel fileModel = null;
            RecordFile file = null;
            var entity = await _repo.Query().SingleOrDefaultAsync(q => q.Id == commandModel.Id, cancellationToken);
            if (Config.FileOperations && _repo.ReflectionRecordModel.HasFile && commandModel is RecordFileModel)
            {
                fileModel = commandModel as RecordFileModel;
                if (_fileUtil.CheckFile(fileModel.FormFile) == false)
                    return Error((Messages as MessagesModel).InvalidFileExtensionOrFileLength);
                file = entity as RecordFile;
                _fileUtil.UpdateFile(fileModel.FormFile, file);
            }
            _repo.Update(_mapperUtil.Map(commandModel, entity));
            try
            {
                await _unitOfWork.SaveAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Error(Messages.RecordNotFound);
            }
            if (fileModel is not null && file is not null)
            {
                _fileUtil.UploadFile(fileModel.FormFile, file);
            }
            return Success(Messages.UpdatedSuccessfully, commandModel.Id);
        }

        public override async Task<Response> Delete(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repo.Query().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
            _fileUtil.DeleteFile(entity);
            _repo.Delete(entity);
            await _unitOfWork.SaveAsync(cancellationToken);
            return Success(Messages.DeletedSuccessfully);
        }

        public virtual async Task<FileToDownloadModel> DownloadFile(int id, string fileToDownloadFileNameWithoutExtension = null, bool useOctetStreamContentType = false, CancellationToken cancellationToken = default)
        {
            FileToDownloadModel file = _fileUtil.GetFile(id, fileToDownloadFileNameWithoutExtension, useOctetStreamContentType);
            if (file is null)
            {
                if (_repo.ReflectionRecordModel.HasFile)
                {
                    var entity = await _repo.Query().SingleOrDefaultAsync(q => q.Id == id, cancellationToken);
                    if (entity is null)
                        return null;
                    var fileDataPropertyInfo = _reflectionUtil.GetPropertyInfo(entity, _repo.ReflectionRecordModel.FileData);
                    var fileData = fileDataPropertyInfo?.GetValue(entity);
                    var fileContentPropertyInfo = _reflectionUtil.GetPropertyInfo(entity, _repo.ReflectionRecordModel.FileContent);
                    var fileContent = fileContentPropertyInfo?.GetValue(entity);
                    file = new FileToDownloadModel()
                    {
                        FileStream = fileData is not null ? new MemoryStream((byte[])fileData) : null,
                        FileContentType = fileContent is not null ? _fileUtil.GetContentType(fileContent.ToString(), false, false) : null,
                        FileName = fileContent is not null ?
                            (string.IsNullOrWhiteSpace(fileToDownloadFileNameWithoutExtension) ? id + fileContent.ToString() :
                                fileToDownloadFileNameWithoutExtension + fileContent.ToString())
                            : null
                    };
                }
            }
            return file;
        }

        public virtual async Task<Response> DeleteFile(int id, CancellationToken cancellationToken = default)
        {
            if (!Config.FileOperations)
                return Error((Messages as MessagesModel).FileOperationsNotConfigured);
            var entity = await _repo.Query().SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
            _fileUtil.DeleteFile(entity);
            _repo.Update(entity);
            await _unitOfWork.SaveAsync(cancellationToken);
            return Success((Messages as MessagesModel).FileDeletedSuccessfully, id);
        }

        public virtual async Task ExportToExcel(string fileNameWithoutExtension)
        {
            _reportUtil.Set(Config.IsExcelLicenseCommercial, Language);
            _reportUtil.ExportToExcel(await GetList(), fileNameWithoutExtension);
        }

        public virtual async Task ExportToExcel(string fileNameWithoutExtension, PageOrderFilterModel pageOrderFilterModel)
        {
            _reportUtil.Set(Config.IsExcelLicenseCommercial, Language);
            _reportUtil.ExportToExcel(await GetList(pageOrderFilterModel), fileNameWithoutExtension);
        }
    }

    public abstract class ServiceBase<TEntity, TModel> : ServiceBase<TEntity, TModel, TModel>, IService<TModel, TModel> where TEntity : Record, new() where TModel : Record, new()
    {
        protected ServiceBase(UnitOfWorkBase unitOfWork, RepoBase<TEntity> repo, ReflectionUtilBase reflectionUtil, CultureUtilBase cultureUtil, SessionUtilBase sessionUtil, 
            MapperUtilBase<TEntity, TModel, TModel> mapperUtil, FileUtilBase fileUtil, ReportUtilBase reportUtil) : base(unitOfWork, repo, reflectionUtil, cultureUtil, sessionUtil, mapperUtil, fileUtil, reportUtil)
        {
        }
    }
}
