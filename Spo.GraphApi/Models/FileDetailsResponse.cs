using System.Text.Json.Serialization;

namespace Spo.GraphApi.Models;

public class FileDetailsResponse
{
    [JsonPropertyName("@odata.context")]
    public string odatacontext { get; set; }
    public List<FileDetails> value { get; set; }
}
