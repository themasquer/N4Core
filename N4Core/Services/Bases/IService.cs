using N4Core.Files.Models;
using N4Core.Responses.Bases;
using N4Core.Services.Configs;
using N4Core.Services.Models;

namespace N4Core.Services.Bases
{
    public interface IService<TQueryModel, TCommandModel> : ICrudService<TQueryModel, TCommandModel>
    {
        public void Set(Action<ServiceConfig> config);
        public IQueryable<TQueryModel> Query(PageOrderFilterModel pageModel);
        public Task<List<TQueryModel>> GetList(PageOrderFilterModel pageModel, CancellationToken cancellationToken = default);
        public Task<FileToDownloadModel> DownloadFile(int id, string fileToDownloadFileNameWithoutExtension = null, bool useOctetStreamContentType = false, CancellationToken cancellationToken = default);
        public Task<Response> DeleteFile(int id, CancellationToken cancellationToken = default);
        public Task ExportToExcel(string fileNameWithoutExtension);
        public Task ExportToExcel(string fileNameWithoutExtension, PageOrderFilterModel pageOrderFilterModel);
    }
}
