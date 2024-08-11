namespace Valetax.Domain.Models.Exceptions;

public class ErrorDetails
{
    public string? Type { get; set; }
    public string? Id { get; set; }
    public ErrorDetailsData Data { get; set; }
}