using Microsoft.AspNetCore.Mvc;
using N4Core.ArtificialIntelligence.Models;
using N4Core.ArtificialIntelligence.Utils.Bases;
using N4Core.Controllers.Bases;
using N4Core.Cookie.Utils.Bases;
using N4Core.Culture.Utils.Bases;
using N4Core.Session.Utils.Bases;

namespace N4Core.ArtificialIntelligence.Controllers
{
    public abstract class AiMvcController : MvcController
    {
        protected readonly IAiUtil _aiUtil;

        public AiMvcController(CultureUtilBase cultureUtil, CookieUtilBase cookieUtil, SessionUtilBase sessionUtil, IAiUtil aiUtil) : base(cultureUtil, cookieUtil, sessionUtil)
        {
            _aiUtil = aiUtil;
            _aiUtil.Set(_cultureUtil.GetLanguage());
        }

        public virtual IActionResult Index()
        {
            return View(new AiViewModel());
        }

        [HttpPost]
        public virtual async Task<IActionResult> Index(AiViewModel viewModel)
        {
            return View(nameof(Index), await _aiUtil.Prompt(viewModel));
        }
    }
}
