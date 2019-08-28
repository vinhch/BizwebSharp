# BizwebSharp

[![Nuget](https://img.shields.io/nuget/v/BizwebSharp?maxAge=3600)](https://www.nuget.org/packages/BizwebSharp/)

BizwebSharp is a C# and .NET client library for Bizweb.vn API, and now is Sapo.vn web store API.

Inspired by [ShopifySharp](https://github.com/nozzlegear/ShopifySharp), [Stripe.net](https://github.com/jaymedavis/stripe.net) and [GeckoApi](https://github.com/ElijahGlover/GeckoApi).

This project's still drafts. if you want to contribute, feel free to open an issue. That is appreciated.

# Installation
BizwebSharp is [available on NuGet](https://www.nuget.org/packages/BizwebSharp/). Use the package manager console in Visual Studio to install it:

```
Install-Package BizwebSharp
```

If you're using .NET Core, you can use the `dotnet` command from your favorite shell:

```
dotnet add package bizwebsharp
```

# How to use
The official document about authorization and authentication at Bizweb can be found in [here](https://support.sapo.vn/oauth). I recommend you should read all of them before using this client.

First you need to create a Bizweb app, get the API Key and Secret Key in [here](https://developers.bizweb.vn/services/partners/api_clients).

Then perform authorization action, you need to build an authorization url with the ``store_name``, ``api_key``, ``scopes`` and ``redirect_uri``. The full ``scopes`` are listed in [here](https://support.sapo.vn/oauth#scopes). BizwebSharp has supported ``AuthorizationService.BuildAuthorizationUrl`` method to build authorization url.

### Build an authorization URL

Redirect your users to this authorization URL, where they'll be prompted to install your app to their Bizweb store.

```c#
//This is the user's store URL.
string usersMyBizwebUrl = "https://example.bizwebvietnam.net";

// A URL to redirect the user to after they've confirmed app installation.
// This URL is required, and must be listed in your app's settings in your Bizweb app dashboard.
// It's case-sensitive too!
string redirectUrl = "https://example.com/my/redirect/url";

//An array of the Bizweb access scopes your application needs to run.
var scopes = new List<AuthorizationScope>()
{
    AuthorizationScope.ReadCustomers,
    AuthorizationScope.WriteCustomers
};

//Or, use an array of string permissions
var scopes = new List<string>()
{
    "read_customers",
    "write_customers"
}

//All AuthorizationService methods are static.
string authUrl = AuthorizationService.BuildAuthorizationUrl(scopes, usersMyBizwebUrl, bizwebApiKey, redirectUrl);
```

### Authorize an installation and generate an access token

Once you've sent a user to the authorization URL and they've confirmed your app installation, they'll be redirected
back to your application at either the default app URL, or the redirect URL you passed in when building the
authorization URL.

The access token you receive after authorizing should be stored in your database. You'll need it to access the
shop's resources (e.g. orders, customers, fulfillments, etc.)

```c#
//The querystring will have several parameters you need for authorization.
string code = Request.QueryString["code"];
string myBizwebUrl = Request.QueryString["shop"];

string accessToken = await AuthorizationService.AuthorizeAsync(code, myBizwebUrl, bizwebApiKey, bizwebSecretKey);
```

### Using a BizwebSharp API service with ``access_token``

Once you have the ``access_token``, now you can build authorization state object, ``BizwebAuthorizationState``,  that contains the Bizweb API infomations, including API [access_token](https://support.sapo.vn/oauth#confirming-installation). Then you can pass authorization state object to services and use them to query to Bizweb API

```c#
var authState = new BizwebAuthorizationState()
{
    ApiUrl = "https://example.bizwebvietnam.net",
    AccessToken = shopAccessToken
};

var storeService = new StoreService(authState);

var store = await storeService.GetAsync();

var productService = new ProductService(authState);

var createdProduct = await productService.CreateAsync(product);

var createdProductId = createdProduct.Id;

// and many more operations...
```

### Determine if a request is authentic

Any (non-webhook, non-proxy-page) request coming from Bizweb will have a querystring parameter called 'hmac' that you can use
to verify that the request is authentic. This signature is a hash of all querystring parameters and your app's
secret key.

Pass the entire querystring to `AuthorizationService` to verify the request.

```c#
var qs = Request.QueryString;

if(AuthorizationService.IsAuthenticRequest(qs, bizwebSecretKey))
{
    //Request is authentic.
}
else
{
    //Request is not authentic and should not be acted on.
}
```

### Determine if a proxy page request is authentic

Nearly identical to authenticating normal requests, a proxy page request only differs in the way the HMAC is generated. All proxy page requests coming from Bizweb will have a querystring parameter named `hmac` that you can use to verify the request. This signature is a hash of all querystring parameters and your app's secret key.

```cs
var qs = Request.QueryString;

if(AuthorizationService.IsAuthenticProxyRequest(qs, bizwebSecretKey))
{
    //Request is authentic.
}
else
{
    //Request is not authentic and should not be acted on.
}
```
### Determine if a webhook request is authentic

Any webhook request coming from Bizweb will have a header called `X-Bizweb-Hmac-SHA256` that you can use
to verify that the webhook is authentic. The header is a hash of the entire request body and your app's
secret key.

Pass the entire header collection and the request's input stream to `AuthorizationService` to verify
the request.

```c#
NameValueCollection requestHeaders = Request.Headers;
Stream inputStream = Request.InputStream;

if(AuthorizationService.IsAuthenticWebhook(requestHeaders, inputStream, bizwebSecretKey))
{
    //Webhook is authentic.
}
else
{
    //Webhook is not authentic and should not be acted on.
}
```

You can also pass in the request body as a string, rather than using the input stream. However, the request
body string **needs to be identical to the way it was sent from Bizweb**. If it has been modified the verification will fail -- even if just one space is in the wrong place.

```c#
NameValueCollection requestHeaders = Request.Headers;
string requestBody = null;

//Reset the input stream. MVC controllers often read the stream to determine which parameters to pass to an action.
Request.InputStream.Position = 0;

//Read the stream into a string
using(StreamReader reader = new StreamReader(Request.InputStream))
{
    requestBody = await reader.ReadToEndAsync();
}

if(AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, bizwebSecretKey))
{
    //Webhook is authentic.
}
else
{
    //Webhook is not authentic and should not be acted on.
}

```