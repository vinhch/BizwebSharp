using System.Threading.Tasks;
using RestSharp.Portable;

namespace BizwebSharp.Infrastructure
{
    public class DefaultRequestExecutionPolicy : IRequestExecutionPolicy
    {
        public async Task<T> Run<T>(IRestClient client, ICustomRestRequest request,
            ExecuteRequestAsync<T> executeRequestAsync)
        {
            return (await executeRequestAsync(client)).Result;
        }

        //public static IRequestExecutionPolicy Default { get; set; } = new DefaultRequestExecutionPolicy();
        public static IRequestExecutionPolicy Default { get; set; } = new LimitRetryExecutionPolicy();
    }
}