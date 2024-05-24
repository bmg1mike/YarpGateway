namespace Sterling.Gateway.Domain;

public class Endpoint
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public ApiType ApiType { get; set; }
    public required string SubUrl { get; set; }
    public MicroService? MicroService { get; set; }
    public required string MicroServiceId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow.AddHours(1);
    public DateTime DateModified { get; set; }  = DateTime.UtcNow.AddHours(1);
}
