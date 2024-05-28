namespace Sterling.Gateway.Domain;

public class RouteConfigEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string RouteId { get; set; }
    public required string ClusterId { get; set; }
    public required string Path { get; set; }
    public string AuthorizationPolicy { get; set; }
    public required string MicroServiceId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow.AddHours(1);
    public DateTime DateModified { get; set; }  = DateTime.UtcNow.AddHours(1);
}
