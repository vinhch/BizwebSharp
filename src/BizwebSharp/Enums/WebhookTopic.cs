using System.Runtime.Serialization;
using BizwebSharp.Converters;
using Newtonsoft.Json;

namespace BizwebSharp.Enums
{
    [JsonConverter(typeof(NullableEnumConverter<WebhookTopic>))]
    public enum WebhookTopic
    {
        [EnumMember(Value = "orders/create")]
        OrderCreated,

        [EnumMember(Value = "orders/delete")]
        OrderDeleted,

        [EnumMember(Value = "orders/updated")]
        OrderUpdated,

        [EnumMember(Value = "orders/paid")]
        OrderPaid,

        [EnumMember(Value = "orders/cancelled")]
        OrderCancelled,

        [EnumMember(Value = "orders/fulfilled")]
        OrderFulfilled,

        [EnumMember(Value = "orders/partially_fulfilled")]
        OrderPartiallyFulfilled,

        [EnumMember(Value = "order_transactions/create")]
        OrderTransactionCreated,

        [EnumMember(Value = "carts/create")]
        CartCreated,

        [EnumMember(Value = "carts/update")]
        CartUpdated,

        [EnumMember(Value = "checkouts/create")]
        CheckoutCreated,

        [EnumMember(Value = "checkouts/update")]
        CheckoutUpdated,

        [EnumMember(Value = "checkouts/delete")]
        CheckoutDeleted,

        [EnumMember(Value = "refunds/create")]
        RefundCreated,

        [EnumMember(Value = "products/create")]
        ProductCreated,

        [EnumMember(Value = "products/update")]
        ProductUpdated,

        [EnumMember(Value = "products/delete")]
        ProductDeleted,

        [EnumMember(Value = "collections/create")]
        CollectionCreated,

        [EnumMember(Value = "collections/update")]
        CollectionUpdated,

        [EnumMember(Value = "collections/delete")]
        CollectionDeleted,

        [EnumMember(Value = "customer_groups/create")]
        CustomerGroupCreated,

        [EnumMember(Value = "customer_groups/update")]
        CustomerGroupUpdated,

        [EnumMember(Value = "customer_groups/delete")]
        CustomerGroupDeleted,

        [EnumMember(Value = "customers/create")]
        CustomerCreated,

        [EnumMember(Value = "customers/enable")]
        CustomerEnabled,

        [EnumMember(Value = "customers/disable")]
        CustomerDisabled,

        [EnumMember(Value = "customers/update")]
        CustomerUpdated,

        [EnumMember(Value = "customers/delete")]
        CustomerDeleted,

        [EnumMember(Value = "fulfillments/create")]
        FulfillmentCreated,

        [EnumMember(Value = "fulfillments/update")]
        FulfillmentUpdated,

        [EnumMember(Value = "shop/update")]
        ShopUpdated,

        [EnumMember(Value = "disputes/create")]
        DisputeCreated,

        [EnumMember(Value = "disputes/update")]
        DisputeUpdated,

        [EnumMember(Value = "app/uninstalled")]
        AppUninstalled,

        [EnumMember(Value = "themes/publish")]
        ThemePublished
    }
}
