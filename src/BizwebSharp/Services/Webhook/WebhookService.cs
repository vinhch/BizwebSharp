using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp
{
    public class WebhookService : BaseServiceWithSimpleCRUD<Webhook, WebhookOption>
    {
        public WebhookService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
