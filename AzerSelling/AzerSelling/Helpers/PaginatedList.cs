namespace AzerSelling.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(List<T> values, int count, int pageSize, int activePage)
        {
            AddRange(values);
            ActivePage = activePage;
            TotalPageCount = (int)Math.Ceiling(count / (double)pageSize);
        }
        public int TotalPageCount { get; set; }
        public int ActivePage { get; set; }
        public bool PreviousPage { get => ActivePage > 1; }
        public bool NextPage { get => ActivePage < TotalPageCount; }

        public static PaginatedList<T> Create(IQueryable<T> query, int pageSize, int page)
        {
            return new PaginatedList<T>(query.Skip((page - 1) * pageSize).Take(pageSize).ToList(), query.Count(), pageSize, page);
        }
    }
}
