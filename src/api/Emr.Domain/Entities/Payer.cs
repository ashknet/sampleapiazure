using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class Payer : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? PayerId { get; private set; } // External payer ID
    public string Type { get; private set; } = string.Empty; // e.g., "Commercial", "Medicare", "Medicaid"
    public string? ContactPhone { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? Website { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public virtual ICollection<Plan> Plans { get; private set; } = new List<Plan>();
    
    protected Payer() { }
    
    public Payer(string name, string code, string type)
    {
        Name = name;
        Code = code;
        Type = type;
        IsActive = true;
    }
    
    public void UpdateDetails(string name, string? payerId, string? contactPhone, string? contactEmail, string? website)
    {
        Name = name;
        PayerId = payerId;
        ContactPhone = contactPhone;
        ContactEmail = contactEmail;
        Website = website;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}