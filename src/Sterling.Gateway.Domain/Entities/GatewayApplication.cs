using Microsoft.AspNetCore.Identity;

namespace Sterling.Gateway.Domain;

public class GatewayApplication : IdentityUser
{
    public string? ApplicationName { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string PermissionRole { get; set; } = Permission.Guest.ToString();
    public DateTime DateCreated { get; set; } = DateTime.UtcNow.AddHours(1);
    public DateTime DateModified { get; set; } = DateTime.UtcNow.AddHours(1);
}
