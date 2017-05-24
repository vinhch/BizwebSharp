using System.Collections.Generic;
using System.Threading.Tasks;
using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class TransactionService : BaseService
    {
        public TransactionService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        public virtual async Task<int> CountAsync(long orderId, ListOption option = null)
        {
            return await MakeRequest<int>($"orders/{orderId}/transactions/count.json", HttpMethod.GET, "count", option);
        }

        public virtual async Task<IEnumerable<Transaction>> ListAsync(long orderId, ListOption option = null)
        {
            return
                await
                    MakeRequest<List<Transaction>>($"orders/{orderId}/transactions.json", HttpMethod.GET, "transactions",
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
                    MakeRequest<Transaction>($"orders/{orderId}/transactions/{transactionId}.json", HttpMethod.GET, "transaction",
                        options);
        }

        public virtual async Task<Transaction> CreateAsync(long orderId, Transaction inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"transaction", inputObject}
            };

            return await MakeRequest<Transaction>($"orders/{orderId}/transactions.json", HttpMethod.POST, "transaction", root);
        }

        public virtual async Task<Transaction> UpdateAsync(long orderId, long transactionId, Transaction inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"transaction", inputObject}
            };
            return
                await
                    MakeRequest<Transaction>($"orders/{orderId}/transactions/{transactionId}.json", HttpMethod.PUT,
                        "transaction", root);
        }

        public virtual async Task DeleteAsync(long orderId, long transactionId)
        {
            await MakeRequest($"orders/{orderId}/transactions/{transactionId}.json", HttpMethod.DELETE);
        }
    }
}
