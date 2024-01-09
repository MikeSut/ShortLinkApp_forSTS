namespace ShortLinks.Presentation.Api.Dto;

public class UrlResponseDto
{
    public string ShortUrl { get; set; } = "";
}

public class CountUrlResponseDto
{
    public string ShortUrl { get; set; } = "";

    public int AmountClicks { get; set; }

}

public class AnonUrlResponseDto
{
    public string Message { get; set; }
    
    public string ShortUrl { get; set; } = "";
}