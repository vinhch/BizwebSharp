using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class TransactionService : BaseService
    {
        public TransactionService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(long orderId, ListOption option = null)
        {
            return await MakeRequestAsync<int>($"orders/{orderId}/transactions/count.json", HttpMethod.Get, "count", option);
        }

        public virtual async Task<IEnumerable<Transaction>> ListAsync(long orderId, ListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<Transaction>>($"orders/{orderId}/transactions.json", HttpMethod.Get, "transactions",
                        option);
        }

        public virtual async Task<Transaction> GetAsync(long orderId, long transactionId, string fields = null)
        {
            dynamic options = null;
            if (!string.IsNullOrEmpty(fields))
            {
                options = new { fields };
            }

            return
                await
                    MakeRequestAsync<Transaction>($"orders/{orderId}/transactions/{transactionId}.json", HttpMethod.Get, "transaction",
                        options);
        }

        public virtual async Task<Transaction> CreateAsync(long orderId, Transaction inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"transaction", inputObject}
            };

            return await MakeRequestAsync<Transaction>($"orders/{orderId}/transactions.json", HttpMethod.Post, "transaction", root);
        }

        public virtual async Task<Transaction> UpdateAsync(long orderId, long transactionId, Transaction inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"transaction", inputObject}
            };
            return
                await
                    MakeRequestAsync<Transaction>($"orders/{orderId}/transactions/{transactionId}.json", HttpMethod.Put,
                        "transaction", root);
        }

        public virtual async Task DeleteAsync(long orderId, long transactionId)
        {
            await MakeRequestAsync($"orders/{orderId}/transactions/{transactionId}.json", HttpMethod.Delete);
        }
    }
}
