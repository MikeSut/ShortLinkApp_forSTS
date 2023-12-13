namespace app_shortlink.Domain.Entity;

public class table_url
{
    public int urls_id { get; set; }
    
    public string full_url { get; set; }
    
    public string short_url { get; set; }
    
    public int click { get; set; }
    
    public DateTime DateCreate { get; set; }


}