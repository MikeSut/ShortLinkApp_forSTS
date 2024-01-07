namespace ShortLinks.Domain.Entity;

public class IpClient
{
    public int Id { get; set; }
    
    public int UrlId { get; set; }
    public Url? Url { get; set; }

    public string ClientIP { get; set; } = "";

}