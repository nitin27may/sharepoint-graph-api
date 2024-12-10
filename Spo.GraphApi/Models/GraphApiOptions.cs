namespace Spo.GraphApi.Models;

public class GraphApiOptions
{
    public const string GraphApiSettings = "GraphApiSettings";
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string SecretId { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string BaseGraphUri { get; set; } = string.Empty;
    public string BaseSpoSiteUri { get; set; } = string.Empty;
}