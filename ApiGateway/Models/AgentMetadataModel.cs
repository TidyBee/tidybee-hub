using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiGateway.Models
{

    public class AgentMetadataModel
    {
        [JsonPropertyName("id")]
        [Key]
        public int Id { get; set; }
        [JsonPropertyName("json")]
        public string? Json { get; set; }
    }
}