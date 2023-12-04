using System.ComponentModel.DataAnnotations;

namespace auth.Models
{

    public class AgentMetadataModel
    {
        [Key]
        public int Id { get; set; }
        public string? Json { get; set; }
    }
}