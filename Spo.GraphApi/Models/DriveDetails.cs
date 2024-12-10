using System.Text.Json.Serialization;

namespace Spo.GraphApi.Models;

public class DriveDetails
{
    [JsonPropertyName("@odata.context")]
    public string OdataContext { get; set; }
    public List<Drive> value { get; set; }
}

public class Drive
{
    public string description { get; set; }
    public string id { get; set; } //id,name,description,webUrl
    public string name { get; set; }
    public string webUrl { get; set; }
}
