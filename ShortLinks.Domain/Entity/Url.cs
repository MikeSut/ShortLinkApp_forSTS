namespace ShortLinks.Domain.Entity;

public class Url
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public DateTime CreationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string ShortUrl { get; set; } = "";
    public int LifeTimeLink { get; set; } = 5;
    public string Permanent { get; set; } = "No";
    public string FullUrl { get; set; } = "";

    
    public List<IpClient> IpClients { get; set; } = new();


}