using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    /// <summary>
    /// A service for manipulating Webhooks API.
    /// </summary>
    public class WebhookService : BaseServiceWithSimpleCRUD<Webhook, WebhookOption>
    {
        public WebhookService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
