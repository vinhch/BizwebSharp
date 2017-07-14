using System.Threading.Tasks;
using RestSharp.Portable;

namespace BizwebSharp.Infrastructure
{
    public delegate Task<RequestResult<T>> ExecuteRequestAsync<T>(IRestClient client);

    /// <summary>
    ///     Used to specify centralized logic that should run when executing shopify requests.
    ///     It is most useful to implement retry logic, but it can also be used for other concerns (i.e. tracing)
    /// </summary>
    public interface IRequestExecutionPolicy
    {
        Task<T> Run<T>(IRestClient baseClient, ICustomRestRequest request, ExecuteRequestAsync<T> executeRequestAsync);
    }
}