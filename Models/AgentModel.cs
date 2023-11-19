using System.ComponentModel.DataAnnotations;

namespace api.Models
{

    public class AgentModel
    {
        [Key]
        public Guid Uuid { get; }
        public AgentStatusModel Status { get; set; }
        public DateTime LastPing { get; set; } // Will be change by the issue #6
        public AgentMetadataModel? Metadata { get; set; }
        public ConnectionModel? ConnectionInformation { get; set; }
    }
}