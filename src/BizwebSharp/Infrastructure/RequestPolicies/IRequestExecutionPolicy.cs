using System.Threading.Tasks;

namespace BizwebSharp.Infrastructure
{
    public delegate Task<RequestResult<T>> ExecuteRequestAsync<T>(BizwebRequestMessage reqMsg);

    /// <summary>
    ///     Used to specify centralized logic that should run when executing bizweb requests.
    ///     It is most useful to implement retry logic, but it can also be used for other concerns (i.e. tracing)
    /// </summary>
    public interface IRequestExecutionPolicy
    {
        Task<T> Run<T>(BizwebRequestMessage baseReqMsg, ExecuteRequestAsync<T> sendRequestAsync);
    }
}