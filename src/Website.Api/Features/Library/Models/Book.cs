namespace Website.Api.Features.Library.Models;

public class Book
{
    public required string Id { get; set; }
    public required string LibraryId { get; set; }
    public required BookContent Content { get; set; }
}