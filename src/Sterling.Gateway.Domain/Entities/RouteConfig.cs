namespace Sterling.Gateway.Domain;

public class RouteConfigEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string RouteId { get; set; }
    public required string ClusterId { get; set; }
    public required string Path { get; set; }
    public string AuthorizationPolicy { get; set; }
}
