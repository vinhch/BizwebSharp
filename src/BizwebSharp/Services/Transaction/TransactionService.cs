using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Transactions API.
    /// </summary>
    public class TransactionService : BaseService
    {
        public TransactionService(BizwebAuthorizationState authState) : base(authState)
        {
        }

        /// <summary>
        /// Gets a count of all of the shop's transactions.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="option">Options for filtering the results.</param>
        /// <returns>The count of all fulfillments for the shop.</returns>
        public virtual async Task<int> CountAsync(long orderId, ListOption option = null)
        {
            return await MakeRequestAsync<int>($"orders/{orderId}/transactions/count.json", HttpMethod.Get, "count", option);
        }

        /// <summary>
        /// Gets a list of up to 250 of the shop's transactions.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="option">Options for filtering the results.</param>
        /// <returns>The list of transactions.</returns>
        public virtual async Task<IEnumerable<Transaction>> ListAsync(long orderId, ListOption option = null)
        {
            return
                await
                    MakeRequestAsync<List<Transaction>>($"orders/{orderId}/transactions.json", HttpMethod.Get, "transactions",
                        option);
        }

        // <summary>
        /// Retrieves the <see cref="Transaction"/> with the given id.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="transactionId">The id of the Transaction to retrieve.</param>
        /// <param name="fields">A comma-separated list of fields to return.</param>
        /// <returns>The <see cref="Transaction"/>.</returns>
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

        /// <summary>
        /// Creates a new <see cref="Transaction"/> of the given kind.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="inputObject">The transaction.</param>
        /// <returns>The new <see cref="Transaction"/>.</returns>
        public virtual async Task<Transaction> CreateAsync(long orderId, Transaction inputObject)
        {
            var root = new Dictionary<string, object>
            {
                {"transaction", inputObject}
            };

            return await MakeRequestAsync<Transaction>($"orders/{orderId}/transactions.json", HttpMethod.Post, "transaction", root);
        }

        /// <summary>
        /// Updates a <see cref="Transaction"/> of the given kind.
        /// </summary>
        /// <param name="orderId">The order id to which the fulfillments belong.</param>
        /// <param name="transactionId">The id of the Transaction to retrieve.</param>
        /// <param name="inputObject">The transaction.</param>
        /// <returns>The new <see cref="Transaction"/>.</returns>
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

        /// <summary>
        /// Deletes a <see cref="Transaction"/> with the given key.
        /// </summary>
        /// <param name="orderId">The order id to which the transactions belong.</param>
        /// <param name="transactionId">The id of the Transaction to retrieve.</param>
        public virtual async Task DeleteAsync(long orderId, long transactionId)
        {
            await MakeRequestAsync($"orders/{orderId}/transactions/{transactionId}.json", HttpMethod.Delete);
        }
    }
}
