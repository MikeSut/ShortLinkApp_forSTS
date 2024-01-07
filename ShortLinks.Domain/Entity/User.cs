namespace ShortLinks.Domain.Entity;

public class User {
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public List<Url> Urls { get; set; } = new();

}