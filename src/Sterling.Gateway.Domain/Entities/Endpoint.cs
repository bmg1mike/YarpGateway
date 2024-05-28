using System.ComponentModel.DataAnnotations.Schema;

namespace Sterling.Gateway.Domain;

public class Endpoint
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public ApiType ApiType { get; set; }
    public required string SubUrl { get; set; }
    [ForeignKey("MicroServiceId")]
    public ClusterConfigEntity? MicroService { get; set; }
    public required string MicroServiceId { get; set; }
    public string? RequestPayload { get; set; }
    public required string ResponsePayload { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow.AddHours(1);
    public DateTime DateModified { get; set; }  = DateTime.UtcNow.AddHours(1);
}
