namespace ShortLinks.Domain.Entity;

public class PhoneNumber
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User? User { get; set; }
    public int Phone { get; set; } = 0;
}