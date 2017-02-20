using RestSharp.Portable;

namespace BizwebSharp.Infrastructure
{
    public class RequestResult<T>
    {
        public RequestResult(IRestResponse response, T result)
        {
            Response = response;
            Result = result;
        }

        public IRestResponse Response { get; }
        public T Result { get; }
    }
}