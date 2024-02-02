namespace ShortLinks.Domain.Entity;

public class TgChatId
{
    public int Id { get; init; }
    
    public int UserId { get; init; }
    public User? User { get; init; }
    
    public long ChatId { get; init; }

}