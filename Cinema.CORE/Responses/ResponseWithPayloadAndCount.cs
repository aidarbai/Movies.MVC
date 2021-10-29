namespace Cinema.COMMON.Responses
{
    public class ResponseWithPayloadAndCount<T> : BaseResponse
    {
        public ResponseWithPayloadAndCount()
        {
        }
        public ResponseWithPayloadAndCount(T data, int count) : base()
        {
            TotalCount = count;
            Data = data;
        }
        public T Data { get; set; }
        public int TotalCount { get; set; }
    }
}
