using System.ComponentModel.DataAnnotations;

namespace api.Models
{

    public class ConnectionModel
    {
        [Key]
        public int Id { get; set; }
        public string? Address { get; set; }
        public uint Port { get; set; }
    }
}