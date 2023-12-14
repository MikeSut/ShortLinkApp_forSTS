namespace app_shortlink.Domain.Entity;

public class TableUrl
{
    public int Id { get; set; }
    
    public string FullUrl { get; set; }
    
    public string ShortUrl { get; set; }
    
    public int Clicks { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime DateCreate { get; set; }


}