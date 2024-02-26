using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiGateway.Models
{

    public class ConnectionModel
    {
        [JsonPropertyName("id")]
        [Key]
        public int Id { get; set; }
        [JsonPropertyName("address")]
        public string? Address { get; set; }
        [JsonPropertyName("port")]
        public uint Port { get; set; }
    }
}