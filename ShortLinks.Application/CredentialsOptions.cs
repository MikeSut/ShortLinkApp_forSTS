namespace ShortLinks.Application;

public class CredentialsOptions {
    public required string Secret { get; init; }
    
    public required string PublicAddress { get; init; }
}