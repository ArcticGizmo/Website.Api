namespace Website.Api.Features.Library.Models;

public class LibraryDefinition
{
    public string? Id { get; set; }

    public required string Name { get; set; }

    public required string OwnerUserId { get; set; }
}