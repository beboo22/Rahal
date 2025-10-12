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
}
