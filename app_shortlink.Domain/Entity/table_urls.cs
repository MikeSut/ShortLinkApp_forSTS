namespace app_shortlink.Domain.Entity;

public class table_urls
{
    public int Id { get; set; }
    
    public string full_url { get; set; }
    
    public string short_url { get; set; }
    
    public int clicks { get; set; }
    
    public DateTime DateCreate { get; set; }


}