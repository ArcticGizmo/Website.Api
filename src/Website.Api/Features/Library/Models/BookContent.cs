namespace Website.Api.Features.Library.Models;

public class BookContent
{
    public string? Isbn { get; set; }
    public required string Title { get; set; }

    public required List<string> Authors { get; set; } = new();

    public string? Series { get; set; }

    public int? BookInSeries { get; set; }

    public List<string> Tags { get; set; } = new();

    public float? Rating { get; set; }

    public string? CoverImageUrl { get; set; }
}