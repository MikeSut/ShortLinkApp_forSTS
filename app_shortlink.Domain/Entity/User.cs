using System.Text.Json.Serialization;
using app_shortlink.Domain.Enum;

namespace app_shortlink.Domain.Entity
{
    public class User: BaseEntity
    {
        public string Name { get; set; }
        
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}