using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Spo.GraphApi.Models;
using System.Text;
using System.Text.Json;

namespace Spo.GraphApi;

internal class GraphApiClient : IGraphApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GraphApiClient> _logger;
    private readonly GraphApiOptions _graphApiOptions;
    private readonly IDistributedCache _distributedCache;

    public GraphApiClient(HttpClient httpClient, GraphApiOptions graphApiOptions, ILogger<GraphApiClient> logger, IDistributedCache distributedCache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _graphApiOptions = graphApiOptions;
        _distributedCache = distributedCache;
    }


    public async Task<List<FileDetails>> GetAllFiles(string siteName, string driveName, string path, string selectQuery = "id,name,size,webUrl,createdDateTime,lastModifiedDateTime,parentReference", CancellationToken cancellationToken = default)
    {
        var driveDetails = await GetDrive(siteName, driveName, cancellationToken);

        if (driveDetails == null)
            throw new InvalidOperationException($"Drive '{driveName}' not found in site '{siteName}'.");

        var endpoint = $"drives/{driveDetails.id}/items/root:/{path}:/children?$select={selectQuery}";
        var result = await GetAsync<FileDetailsResponse>(endpoint, cancellationToken);
        return result.value;
    }

    public async Task<FileDetails> AddFile(string siteName, string driveName, string path, CustomFile file, CancellationToken cancellationToken = default)
    {
        var driveDetails = await GetDrive(siteName, driveName, cancellationToken);

        if (driveDetails == null)
            throw new InvalidOperationException($"Drive '{driveName}' not found in site '{siteName}'.");

        await using var memoryStream = new MemoryStream();
        await file.File.CopyToAsync(memoryStream, cancellationToken);

        var endpoint = $"drives/{driveDetails.id}/items/root:/{path}/{file.Name}:/content?@microsoft.graph.conflictBehavior=rename";
        return await UploadAsync<FileDetails>(endpoint, memoryStream.ToArray(), cancellationToken);
    }

    public async Task<FileDetails> UpdateFile(string siteName, string driveName, string path, CustomFile file, CancellationToken cancellationToken = default)
    {
        var driveDetails = await GetDrive(siteName, driveName, cancellationToken);

        if (driveDetails == null)
            throw new InvalidOperationException($"Drive '{driveName}' not found in site '{siteName}'.");

        await using var memoryStream = new MemoryStream();
        await file.File.CopyToAsync(memoryStream, cancellationToken);

        var endpoint = $"drives/{driveDetails.id}/items/root:/{path}/{file.Name}:/content";
        return await UploadAsync<FileDetails>(endpoint, memoryStream.ToArray(), cancellationToken);
    }

    public async Task DeleteFile(string siteName, string driveName, string path, string fileName, CancellationToken cancellationToken = default)
    {
        var driveDetails = await GetDrive(siteName, driveName, cancellationToken);

        if (driveDetails == null)
            throw new InvalidOperationException($"Drive '{driveName}' not found in site '{siteName}'.");

        var endpoint = $"drives/{driveDetails.id}/root:/{path}/{fileName}";
        await DeleteAsync(endpoint, cancellationToken);
    }

    public async Task<FileDetails> ReadFile(string siteName, string driveName, string path, string fileName, string selectQuery = "id,name,size,webUrl,createdDateTime,lastModifiedDateTime,parentReference", CancellationToken cancellationToken = default)
    {
        var driveDetails = await GetDrive(siteName, driveName, cancellationToken);

        if (driveDetails == null)
            throw new InvalidOperationException($"Drive '{driveName}' not found in site '{siteName}'.");

        var endpoint = $"drives/{driveDetails.id}/items/root:/{path}/{fileName}?$select={selectQuery}";
        return await GetAsync<FileDetails>(endpoint, cancellationToken);
    }

    public async Task<FileDetails> UpdateFileMetadata(string siteName, string driveName, string path, string fileName, Dictionary<string, string> metadataUpdates, CancellationToken cancellationToken = default)
    {
        var driveDetails = await GetDrive(siteName, driveName, cancellationToken);

        if (driveDetails == null)
            throw new InvalidOperationException($"Drive '{driveName}' not found in site '{siteName}'.");

        if (metadataUpdates == null || metadataUpdates.Count == 0)
            throw new ArgumentException("Metadata updates cannot be null or empty.", nameof(metadataUpdates));

        var endpoint = $"drives/{driveDetails.id}/root:/{path}/{fileName}";
        return await PatchAsync<Dictionary<string, string>, FileDetails>(endpoint, metadataUpdates, cancellationToken);
    }

    private async Task<SiteDetails> GetSiteId(string siteName, CancellationToken cancellationToken = default)
    {
        var endpoint = string.IsNullOrWhiteSpace(siteName) || siteName.Equals("root", StringComparison.OrdinalIgnoreCase)
       ? $"sites/{_graphApiOptions.BaseSpoSiteUri}"
       : $"sites/{_graphApiOptions.BaseSpoSiteUri}:/sites/{siteName}";
        var siteIdByteArray = await _distributedCache.GetAsync(endpoint, cancellationToken);
        if (siteIdByteArray?.Length > 0)
        {
            return JsonSerializer.Deserialize<SiteDetails>(Encoding.UTF8.GetString(siteIdByteArray));
        }

        var siteDetails = await GetAsync<SiteDetails>(endpoint);

        DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0, 0));
        _distributedCache.Set(endpoint, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(siteDetails)), cacheEntryOptions);

        return siteDetails;
    }

    private async Task<Drive?> GetDrive(string siteName, string DriveName, CancellationToken cancellationToken = default)
    {
        var driveDetailsByteArray = await _distributedCache.GetAsync(siteName + DriveName, cancellationToken);
        if (driveDetailsByteArray?.Length > 0)
        {
            return JsonSerializer.Deserialize<Drive>(Encoding.UTF8.GetString(driveDetailsByteArray));
        }

        var siteDetails = await GetSiteId(siteName);
        var drives = (await GetAsync<DriveDetails>($"/sites/{siteDetails.id}/drives?$select=id,name,description,webUrl")).value;
        var driveDetail = drives?.FirstOrDefault(x => x.name == DriveName);

        DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0, 0));
        _distributedCache.Set(siteName + DriveName, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(driveDetail)), cacheEntryOptions);

        return driveDetail;
    }

    public async Task<List<Drive>> GetDriveItems(string siteId, string driveId)
    {
        return (await GetAsync<DriveDetails>($"/sites/{siteId}/drives/{driveId}/items/root/children?$select=id,name,description,webUrl")).value;
    }

    private async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _httpClient
            .GetAsync($"{_graphApiOptions.BaseGraphUri}/{endpoint}", cancellationToken);
        var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            _logger.LogError("Error calling Endpoint: GET {@Endpoint}. Response: {@Response}.",
                responseMessage.RequestMessage.RequestUri, content);

            throw new GraphApiException(responseMessage.StatusCode, content);
        }

        return JsonSerializer.Deserialize<T>(content);
    }

    private async Task<TOut> PostAsync<TIn, TOut>(string endpoint, TIn data, CancellationToken cancellationToken = default)
    {
        var stringContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        HttpResponseMessage responseMessage = await _httpClient
            .PostAsync($"{_graphApiOptions.BaseGraphUri}/{endpoint}", stringContent, cancellationToken);
        var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            var requestMessage = await responseMessage.RequestMessage.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Error calling Endpoint: POST {@Endpoint}. Request: {@Request}, Response: {@Response}.",
                responseMessage.RequestMessage.RequestUri, requestMessage, content);

            throw new GraphApiException(responseMessage.StatusCode, content);
        }

        return JsonSerializer.Deserialize<TOut>(content);
    }

    private async Task<TOut> PutAsync<TIn, TOut>(string endpoint, TIn data, CancellationToken cancellationToken = default)
    {
        var stringContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        HttpResponseMessage responseMessage = await _httpClient
            .PutAsync($"{_graphApiOptions.BaseGraphUri}/{endpoint}", stringContent, cancellationToken);
        var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            var requestMessage = await responseMessage.RequestMessage.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Error calling Endpoint: POST {@Endpoint}. Request: {@Request}, Response: {@Response}.",
                responseMessage.RequestMessage.RequestUri, requestMessage, content);

            throw new GraphApiException(responseMessage.StatusCode, content);
        }

        return JsonSerializer.Deserialize<TOut>(content);
    }

    private async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage responseMessage = await _httpClient
            .DeleteAsync($"{_graphApiOptions.BaseGraphUri}/{endpoint}", cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            var requestUri = responseMessage.RequestMessage?.RequestUri?.ToString();
            var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError("Error calling Endpoint: DELETE {@Endpoint}. Response: {@Response}.",
                requestUri, content);

            throw new GraphApiException(responseMessage.StatusCode, content);
        }
    }

    private async Task<TOut> UploadAsync<TOut>(string endpoint, byte[] data, CancellationToken cancellationToken = default)
    {
        var multiPartData = new ByteArrayContent(data);
        _httpClient.DefaultRequestHeaders.Add("ContentType", "application/octet-stream");
        HttpResponseMessage responseMessage = await _httpClient
            .PutAsync($"{_graphApiOptions.BaseGraphUri}/{endpoint}", multiPartData, cancellationToken);
        var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            var requestMessage = await responseMessage.RequestMessage.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Error calling Endpoint: POST {@Endpoint}. Request: {@Request}, Response: {@Response}.",
                responseMessage.RequestMessage.RequestUri, requestMessage, content);

            throw new GraphApiException(responseMessage.StatusCode, content);
        }

        return JsonSerializer.Deserialize<TOut>(content);
    }

    private async Task<TOut> PatchAsync<TIn, TOut>(string endpoint, TIn data, CancellationToken cancellationToken = default)
    {
        try
        {
            var stringContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_graphApiOptions.BaseGraphUri}/{endpoint}")
            {
                Content = stringContent
            };
            HttpResponseMessage responseMessage = await _httpClient.SendAsync(request, cancellationToken);
            var content = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger.LogError("Error calling Endpoint: PATCH {@Endpoint}. Request: {@Request}, Response: {@Response}.",
                    responseMessage.RequestMessage.RequestUri, data, content);

                throw new GraphApiException(responseMessage.StatusCode, content);
            }

            return JsonSerializer.Deserialize<TOut>(content);
        }
        catch (Exception ex)
        {

            throw ex;
        }

    }

}