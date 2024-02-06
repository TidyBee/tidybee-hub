using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiGateway.Models
{

    public class AgentModel
    {
        [JsonPropertyName("uuid")]
        [Key]
        public Guid Uuid { get; set; }
        [JsonPropertyName("status")]
        public AgentStatusModel Status { get; set; }
        [JsonPropertyName("lastPing")]
        public DateTime LastPing { get; set; } // Will be change by the issue #6
        [JsonPropertyName("metadata")]
        public AgentMetadataModel? Metadata { get; set; }
        [JsonPropertyName("connectionInformation")]
        public ConnectionModel? ConnectionInformation { get; set; }
    }
}