#nullable disable

using Microsoft.AspNetCore.Mvc;
using N4Core.Controllers.Bases;
using N4Core.Cookie.Utils.Bases;
using N4Core.Culture.Utils.Bases;
using N4Core.Files.Enums;
using N4Core.Files.Models;
using N4Core.Files.Services.Bases;
using N4Core.Session.Utils.Bases;
using N4Core.Views.Models;

namespace N4Core.Files.Controllers
{
    public abstract class FileBrowserMvcController : MvcController
    {
        protected readonly FileBrowserServiceBase _fileBrowserService;

        protected FileBrowserMvcController(FileBrowserServiceBase fileBrowserService, CultureUtilBase cultureUtil, CookieUtilBase cookieUtil, SessionUtilBase sessionUtil) : base(cultureUtil, cookieUtil, sessionUtil)
        {
            _fileBrowserService = fileBrowserService;
        }

        public virtual async Task<IActionResult> Index(FileBrowserModel model)
        {
            var viewModel = await _fileBrowserService.GetContents(model);
            if (viewModel is null)
                return View("Error", new ViewErrorModel(_fileBrowserService.Language));
            if (viewModel.FileType == FileTypes.Other)
                return File(viewModel.FileBinaryContent, viewModel.FileContentType, viewModel.Title);
            return View(viewModel);
        }

        public virtual async Task<IActionResult> Sync(bool reset = false)
        {
            var result = await _fileBrowserService.SyncFileBrowserItemsWithDatabase(reset);
            return View(nameof(Index), new FileBrowserModel()
            {
                Operation = FileBrowserOperations.Sync,
                OperationMessage = result.Message,
                Filter = new FileBrowserFilterModel()
                {
                    Language = _fileBrowserService.Language
                }
            });
        }
    }
}
