namespace Sterling.Gateway.Domain;

public class ClusterConfigEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string ClusterId { get; set; }
    public required string DestinationAddress { get; set; }
}
