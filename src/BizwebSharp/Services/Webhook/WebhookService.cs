using BizwebSharp.Entities;
using BizwebSharp.Infrastructure;
using BizwebSharp.Options;

namespace BizwebSharp.Services
{
    public class WebhookService : BaseServiceWithSimpleCRUD<Webhook, WebhookOption>
    {
        public WebhookService(BizwebAuthorizationState authState) : base(authState)
        {
        }
    }
}
