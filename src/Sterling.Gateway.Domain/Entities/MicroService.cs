namespace Sterling.Gateway.Domain;

public class MicroService
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string MicroServiceName { get; set; }
    public required string BaseUrl { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow.AddHours(1);
    public DateTime DateModified { get; set; }  = DateTime.UtcNow.AddHours(1);
    
}
