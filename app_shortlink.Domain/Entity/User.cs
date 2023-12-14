using System.Text.Json.Serialization;
using app_shortlink.Domain.Enum;

namespace app_shortlink.Domain.Entity
{
    public class User: BaseEntity
    {
        public string Name { get; set; }
        public string Password { get; set; }
        
        public string Email { get; set; }
        [JsonIgnore]
        
        public Role Role { get; set; }
    }
}