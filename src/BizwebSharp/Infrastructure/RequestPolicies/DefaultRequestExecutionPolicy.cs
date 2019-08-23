using System.Net.Http;
using System.Threading.Tasks;

namespace BizwebSharp.Infrastructure
{
    public class DefaultRequestExecutionPolicy : IRequestExecutionPolicy
    {
        public async Task<T> Run<T>(BizwebRequestMessage requestMsg,
            ExecuteRequestAsync<T> executeRequestAsync)
        {
            return (await executeRequestAsync(requestMsg)).Result;
        }

        private static IRequestExecutionPolicy _default = new LimitRetryExecutionPolicy();
        //public static IRequestExecutionPolicy Default { get; set; } = new DefaultRequestExecutionPolicy();
        public static IRequestExecutionPolicy Default
        {
            get => _default ?? (_default = new LimitRetryExecutionPolicy());
            set
            {
                if (value == null)
                {
                    _default = new LimitRetryExecutionPolicy();
                    return;
                }
                _default = value;
            }
        }
    }
}