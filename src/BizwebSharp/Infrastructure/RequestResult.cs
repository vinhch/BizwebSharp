using System.Net.Http;

namespace BizwebSharp.Infrastructure
{
    public class RequestResult<T>
    {
        public RequestResult(HttpResponseMessage response, T result)
        {
            Response = response;
            Result = result;
        }

        public HttpResponseMessage Response { get; }
        public T Result { get; }
    }
}