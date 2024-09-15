#nullable disable

using Microsoft.AspNetCore.Http;
using N4Core.Culture;
using N4Core.Culture.Utils.Bases;
using N4Core.Reflection.Utils.Bases;
using N4Core.Types.Extensions;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace N4Core.Reports.Utils.Bases
{
    public abstract class ReportUtilBase
    {
        protected readonly ReflectionUtilBase _reflectionUtil;
        protected readonly CultureUtilBase _cultureUtil;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected Languages _language;
        protected bool _isExcelLicenseCommercial;

        protected ReportUtilBase(ReflectionUtilBase reflectionUtil, CultureUtilBase cultureUtil, IHttpContextAccessor httpContextAccessor)
        {
            _reflectionUtil = reflectionUtil;
            _cultureUtil = cultureUtil;
            _httpContextAccessor = httpContextAccessor;
            _language = _cultureUtil.GetLanguage();
        }

        public void Set(bool isExcelLicenseCommercial, Languages? language = null)
        {
            _isExcelLicenseCommercial = isExcelLicenseCommercial;
            if (language.HasValue)
                _language = language.Value;
        }

        public virtual void ExportToExcel<TModel>(List<TModel> list, string fileNameWithoutExtension) where TModel : class, new()
        {
            var data = ConvertToByteArrayForExcel(list);
            if (data is not null && data.Length > 0)
            {
                _httpContextAccessor.HttpContext.Response.Headers.Clear();
                _httpContextAccessor.HttpContext.Response.Clear();
                _httpContextAccessor.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                _httpContextAccessor.HttpContext.Response.Headers.Append("content-length", data.Length.ToString());
                _httpContextAccessor.HttpContext.Response.Headers.Append("content-disposition", "attachment; filename=\"" + fileNameWithoutExtension + ".xlsx\"");
                _httpContextAccessor.HttpContext.Response.Body.WriteAsync(data, 0, data.Length);
                _httpContextAccessor.HttpContext.Response.Body.Flush();
            }
        }

        protected byte[] ConvertToByteArrayForExcel<TModel>(List<TModel> list) where TModel : class, new()
        {
            byte[] data = null;
            if (list is not null && list.Any())
            {
                var dataTable = _reflectionUtil.ConvertToDataTable(list);
                if (dataTable is not null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        dataTable.Columns[i].ColumnName = dataTable.Columns[i].ColumnName.GetDisplayName(_language);
                    }
                    ExcelPackage.LicenseContext = _isExcelLicenseCommercial ? LicenseContext.Commercial : LicenseContext.NonCommercial;
                    ExcelPackage excelPackage = new ExcelPackage();
                    ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add(_language == Languages.English ? "Sheet1" : "Sayfa1");
                    excelWorksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                    excelWorksheet.Cells["A:AZ"].AutoFitColumns();
                    data = excelPackage.GetAsByteArray();
                }
            }
            return data;
        }
    }
}
