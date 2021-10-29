namespace Cinema.COMMON.Responses
{
    public class ResponseWithPayload<T> : BaseResponse
    {
        public ResponseWithPayload()
        {
        }
        public ResponseWithPayload(T data)
        {
            Data = data;
        }
        public T Data { get; set; }
    }
}
