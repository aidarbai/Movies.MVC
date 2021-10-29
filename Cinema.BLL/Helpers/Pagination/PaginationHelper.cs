using Cinema.COMMON.Filters;
using Cinema.COMMON.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Cinema.BLL.Helpers.Pagination
{
    public static class PaginationHelper
    {
        public static PaginatedResponse<T> GetPaginatedList<T>(ViewFilter filter, IQueryable<T> query) where T : class
        {
            filter.PageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var response = new PaginatedResponse<T>();
            try
            {
                int count = query.Count();
                int totalPages = (int)Math.Ceiling(decimal.Divide(count, filter.PageSize));
                totalPages = totalPages > 0 ? totalPages : 1;

                if (filter.PageNumber > totalPages)
                {
                    filter.PageNumber = totalPages;
                }

                var result = query.AsSingleQuery().Skip((filter.PageNumber - 1) * filter.PageSize)
                                          .Take(filter.PageSize).ToList();

                response.Items = result;
                response.PagesCount = totalPages;
                response.CurrentPage = filter.PageNumber;
            }

            catch (Exception ex)
            {
                response.Succeeded = false;
                response.Ex = ex;
            }

            return response;
        }

    }
}
