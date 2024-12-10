using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spo.GraphApi.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Spo.GraphApi.Handler;

internal class GraphApiAuthenticationHandler : DelegatingHandler
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<GraphApiAuthenticationHandler> _logger;
    private readonly GraphApiOptions _graphApiOptions;

    public GraphApiAuthenticationHandler(
        IDistributedCache distributedCache,
        ILogger<GraphApiAuthenticationHandler> logger,
        IOptions<GraphApiOptions> options)
    {
        _distributedCache = distributedCache;
        _logger = logger;
        _graphApiOptions = options.Value;
        InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string accessToken = await GetAccessTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var tokenByteArray = await _distributedCache.GetAsync("ApplicationCacheKeys.GrapApiToken", cancellationToken);
        if (tokenByteArray?.Length > 0)
        {
            return Encoding.UTF8.GetString(tokenByteArray);
        }
        string[] scopes = new[] { $"{_graphApiOptions.Scope}" };

        ClientSecretCredential clientSecretCredential = new ClientSecretCredential(
                        _graphApiOptions.TenantId,
                        _graphApiOptions.ClientId,
                        _graphApiOptions.SecretId);
        TokenRequestContext tokenRequestContext = new TokenRequestContext(scopes);
        AccessToken tokenResponse = await clientSecretCredential.GetTokenAsync(tokenRequestContext, cancellationToken);

        TimeSpan expirationTime = (tokenResponse.ExpiresOn.UtcDateTime - DateTime.UtcNow).Subtract(TimeSpan.FromMinutes(3));
        DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(expirationTime);
        _distributedCache.Set("ApplicationCacheKeys.GrapApiToken", Encoding.UTF8.GetBytes(tokenResponse.Token), cacheEntryOptions);

        return tokenResponse.Token;
    }
}