using System.Text.Json.Serialization;

namespace api.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortList
    {
        CreateAsc, 
        CreateDesc
    }
}
