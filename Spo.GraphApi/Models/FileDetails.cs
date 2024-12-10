using System.Text.Json.Serialization;

namespace Spo.GraphApi.Models;

public class FileDetails
{
    [JsonPropertyName("@odata.etag")]
    public string odataetag { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("size")]
    public long? Size { get; set; }

    [JsonPropertyName("webUrl")]
    public string WebUrl { get; set; }

    [JsonPropertyName("createdDateTime")]
    public DateTime? CreatedDateTime { get; set; }

    [JsonPropertyName("lastModifiedDateTime")]
    public DateTime? LastModifiedDateTime { get; set; }
    [JsonPropertyName("folder")]
    public FolderDetails Folder { get; set; }

    // This property will be null if the item is not a file
    [JsonPropertyName("file")]
    public FileDetailsProperties File { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ParentReference ParentReference { get; set; }
}
public class ParentReference
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("path")]
    public string Path { get; set; }
}

public class FolderDetails
{
    [JsonPropertyName("childCount")]
    public int ChildCount { get; set; } // Example property for folders
}

public class FileDetailsProperties
{
    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; } // Example property for files
}