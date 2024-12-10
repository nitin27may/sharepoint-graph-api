using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Spo.GraphApi.Models;

public class CustomFile
{
    public string Name { get; set; }
    public IFormFile File { get; set; }
}

public class FileResponse
{
    [JsonPropertyName("@odata.context")]
    public string odatacontext { get; set; }

    [JsonPropertyName("@microsoft.graph.downloadUrl")]
    public string microsoftgraphdownloadUrl { get; set; }

    public DateTime createdDateTime { get; set; }
    public string eTag { get; set; }
    public string id { get; set; }
    public DateTime lastModifiedDateTime { get; set; }
    public string name { get; set; }
    public string webUrl { get; set; }
    public string cTag { get; set; }
    public int size { get; set; }
    public CreatedBy createdBy { get; set; }
    public LastModifiedBy lastModifiedBy { get; set; }
    public FileSystemInfo fileSystemInfo { get; set; }
}

public class User
{
    public string displayName { get; set; }
}

public class Application
{
    public string id { get; set; }
    public string displayName { get; set; }
}

public class CreatedBy
{
    public Application application { get; set; }
    public User user { get; set; }
}

public class FileSystemInfo
{
    public DateTime createdDateTime { get; set; }
    public DateTime lastModifiedDateTime { get; set; }
}

public class LastModifiedBy
{
    public Application application { get; set; }
    public User user { get; set; }
}