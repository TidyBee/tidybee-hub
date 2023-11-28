using System.ComponentModel.DataAnnotations;

namespace api.Models
{

    public class AgentMetadataModel
    {
        [Key]
        public int Id { get; set; }
        public string? Json { get; set; }
    }
}