using System.ComponentModel.DataAnnotations;

namespace auth_server.Models
{
    public class User
    {
        [Key]
        public string? peopleId { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? phoneNumber { get; set; }
        public string? socialName { get; set; }
    }
}
