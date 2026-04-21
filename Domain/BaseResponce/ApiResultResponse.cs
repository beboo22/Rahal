namespace Domain.BaseResponce
{
    public class ApiResultResponse<T> : ApiResponse
    {
        public T? Data { get; set; }
        public ApiResultResponse(int Scode, T? _data, string? msg = null) : base(Scode, msg)
        {
            Data = _data;
        }
    }
    public class PaginationMeta
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
