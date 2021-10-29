using System;

namespace Cinema.COMMON.Responses
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            Succeeded = true;
            Message = string.Empty;
            Ex = null;
        }
        public bool Succeeded { get; set; }
        public Exception Ex { get; set; }
        public string Message { get; set; }
    }
}
