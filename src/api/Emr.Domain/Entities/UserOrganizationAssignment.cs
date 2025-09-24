using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class UserOrganizationAssignment : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsPrimary { get; private set; }
    
    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual Organization Organization { get; private set; } = null!;
    public virtual Department? Department { get; private set; }
    
    protected UserOrganizationAssignment() { }
    
    public UserOrganizationAssignment(Guid userId, Guid organizationId, DateTime startDate, bool isPrimary = false)
    {
        UserId = userId;
        OrganizationId = organizationId;
        StartDate = startDate;
        IsPrimary = isPrimary;
    }
    
    public void AssignToDepartment(Guid departmentId)
    {
        DepartmentId = departmentId;
    }
    
    public void EndAssignment(DateTime endDate)
    {
        EndDate = endDate;
    }
    
    public void SetAsPrimary()
    {
        IsPrimary = true;
    }
    
    public bool IsActive => !EndDate.HasValue || EndDate.Value > DateTime.UtcNow;
}