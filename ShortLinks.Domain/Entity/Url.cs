namespace ShortLinks.Domain.Entity;

public class Url
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public string ShortUrl { get; set; } = "";

    public string FullUrl { get; set; } = "";

    
    public List<IpClient> IpClients { get; set; } = new();


}