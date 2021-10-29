using System;
using System.Collections.Generic;

namespace Cinema.COMMON.Responses
{
    public class PaginatedResponse<T>
        where T : class
    {
        public PaginatedResponse()
        {
            Succeeded = true;
            Ex = null;
        }
        public bool Succeeded { get; set; }
        public Exception Ex { get; set; }
        public List<T> Items { get; set; }
        public int PagesCount { get; set; }
        public int CurrentPage { get; set; }

    }
}
