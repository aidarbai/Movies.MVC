namespace Cinema.COMMON.Filters
{
    public class ViewFilter
    {
        public string SortOrder { get; set; }
        public string SearchString { get; set; }
        public string CurrentFilter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 1;
    }
}
