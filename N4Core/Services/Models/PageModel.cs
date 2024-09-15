using N4Core.Culture;

namespace N4Core.Services.Models
{
    public class PageModel
    {
        public Languages Language { get; set; }
        public int PageNumber { get; set; }
        public string? RecordsPerPageCount { get; set; }
        public int TotalRecordsCount { get; set; }
        public List<string>? RecordsPerPageCounts { get; set; }

        public bool PageSession { get; set; }

        public List<int> PageNumbers
        {
            get
            {
                var pageNumbers = new List<int>();
                if (RecordsPerPageCounts is not null && RecordsPerPageCounts.Any())
                {
                    int recordsPerPageCount;
                    if (TotalRecordsCount > 0 && int.TryParse(RecordsPerPageCount, out recordsPerPageCount))
                    {
                        int numberOfPages = Convert.ToInt32(Math.Ceiling(TotalRecordsCount / Convert.ToDecimal(recordsPerPageCount)));
                        for (int page = 1; page <= numberOfPages; page++)
                        {
                            pageNumbers.Add(page);
                        }
                    }
                    else
                    {
                        pageNumbers.Add(1);
                    }
                }
                else
                {
                    pageNumbers.Add(1);
                }
                return pageNumbers;
            }
        }

        public PageModel()
        {
            PageNumber = 1;
            RecordsPerPageCount = "10";
        }
    }
}
