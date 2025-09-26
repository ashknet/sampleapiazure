using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class Plan : BaseEntity
{
    public Guid PayerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty; // e.g., "HMO", "PPO", "EPO"
    public string? GroupNumber { get; private set; }
    public bool RequiresReferral { get; private set; }
    public bool RequiresPreAuthorization { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public virtual Payer Payer { get; private set; } = null!;
    public virtual ICollection<Coverage> Coverages { get; private set; } = new List<Coverage>();
    
    protected Plan() { }
    
    public Plan(Guid payerId, string name, string code, string type)
    {
        PayerId = payerId;
        Name = name;
        Code = code;
        Type = type;
        IsActive = true;
    }
    
    public void UpdateDetails(string name, string? groupNumber, bool requiresReferral, bool requiresPreAuthorization)
    {
        Name = name;
        GroupNumber = groupNumber;
        RequiresReferral = requiresReferral;
        RequiresPreAuthorization = requiresPreAuthorization;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}