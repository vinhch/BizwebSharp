﻿using System.Net.Http;
using System.Threading.Tasks;

namespace BizwebSharp.Infrastructure
{
    public class DefaultRequestExecutionPolicy : IRequestExecutionPolicy
    {
        public async Task<T> Run<T>(HttpClient client, BizwebRequestMessage requestMsg,
            ExecuteRequestAsync<T> executeRequestAsync)
        {
            return (await executeRequestAsync(client, requestMsg)).Result;
        }

        private static IRequestExecutionPolicy _default = new LimitRetryExecutionPolicy();
        //public static IRequestExecutionPolicy Default { get; set; } = new DefaultRequestExecutionPolicy();
        public static IRequestExecutionPolicy Default
        {
            get => _default ?? (_default = new LimitRetryExecutionPolicy());
            set
            {
                if (value == null) return;
                _default = value;
            }
        }
    }
}