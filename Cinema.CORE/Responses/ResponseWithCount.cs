namespace Cinema.COMMON.Responses
{
    public class ResponseWithCount : BaseResponse
    {
        public ResponseWithCount()
        {
        }
        public ResponseWithCount(int count) : base()
        {
            TotalCount = count;
        }
        public int TotalCount { get; set; }
    }
}
