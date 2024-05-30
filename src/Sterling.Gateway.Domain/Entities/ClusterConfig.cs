namespace Sterling.Gateway.Domain;

public class ClusterConfigEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string ClusterId { get; set; }
    public required string DestinationAddress { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow.AddHours(1);
    public DateTime DateModified { get; set; }  = DateTime.UtcNow.AddHours(1);
}
