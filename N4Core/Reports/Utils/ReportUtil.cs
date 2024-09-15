using Microsoft.AspNetCore.Http;
using N4Core.Culture.Utils.Bases;
using N4Core.Reflection.Utils.Bases;
using N4Core.Reports.Utils.Bases;

namespace N4Core.Reports.Utils
{
    public class ReportUtil : ReportUtilBase
    {
        public ReportUtil(ReflectionUtilBase reflectionUtil, CultureUtilBase cultureUtil, IHttpContextAccessor httpContextAccessor) :
            base(reflectionUtil, cultureUtil, httpContextAccessor)
        {
        }
    }
}
