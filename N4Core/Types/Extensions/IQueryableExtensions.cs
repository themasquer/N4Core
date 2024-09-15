using Microsoft.EntityFrameworkCore;
using N4Core.Records.Bases;
using N4Core.Services.Models;

namespace N4Core.Types.Extensions
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// pageModel's OrderExpression value not ending with "~Desc" is used for ascending order.
        /// Add "~Desc" at the end of the pageModel's OrderExpression value for descending order.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageModel"></param>
        /// <returns>IOrderedQueryable</returns>
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, PageOrderModel pageModel) where TEntity : Record, new()
        {
            if (string.IsNullOrWhiteSpace(pageModel.OrderExpression))
                return query.OrderBy(q => q.Id);
            return pageModel.OrderExpression.EndsWith("~Desc") ? 
                query.OrderByDescending(p => EF.Property<object>(p, pageModel.OrderExpression.Remove(pageModel.OrderExpression.Length - 5))) :
                query.OrderBy(p => EF.Property<object>(p, pageModel.OrderExpression));
        }

        public static IQueryable<TQueryModel> Paginate<TQueryModel>(this IQueryable<TQueryModel> query, PageOrderModel pageModel) where TQueryModel : Record, new()
        {
            pageModel.TotalRecordsCount = query.Count();
            int recordsPerPageCount;
            if (pageModel.RecordsPerPageCounts is not null && pageModel.RecordsPerPageCounts.Any() && int.TryParse(pageModel.RecordsPerPageCount, out recordsPerPageCount))
                query = query.Skip((pageModel.PageNumber - 1) * recordsPerPageCount).Take(recordsPerPageCount);
            return query;
        }
    }
}
