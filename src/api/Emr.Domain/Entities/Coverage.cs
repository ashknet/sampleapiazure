using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class Coverage : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Guid PlanId { get; private set; }
    public string MemberId { get; private set; } = string.Empty;
    public string? GroupNumber { get; private set; }
    public string Relationship { get; private set; } = string.Empty; // e.g., "Self", "Spouse", "Child"
    public int Sequence { get; private set; } // Primary = 1, Secondary = 2, etc.
    public DateTime EffectiveDate { get; private set; }
    public DateTime? TerminationDate { get; private set; }
    public string? SubscriberFirstName { get; private set; }
    public string? SubscriberLastName { get; private set; }
    public DateTime? SubscriberDateOfBirth { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public virtual Patient Patient { get; private set; } = null!;
    public virtual Plan Plan { get; private set; } = null!;
    
    protected Coverage() { }
    
    public Coverage(Guid patientId, Guid planId, string memberId, string relationship, int sequence, DateTime effectiveDate)
    {
        PatientId = patientId;
        PlanId = planId;
        MemberId = memberId;
        Relationship = relationship;
        Sequence = sequence;
        EffectiveDate = effectiveDate;
        IsActive = true;
    }
    
    public void UpdateSubscriberInfo(string firstName, string lastName, DateTime? dateOfBirth)
    {
        SubscriberFirstName = firstName;
        SubscriberLastName = lastName;
        SubscriberDateOfBirth = dateOfBirth;
    }
    
    public void Terminate(DateTime terminationDate)
    {
        TerminationDate = terminationDate;
        IsActive = false;
    }
    
    public void UpdateSequence(int sequence)
    {
        Sequence = sequence;
    }
    
    public bool IsCurrent => IsActive && (!TerminationDate.HasValue || TerminationDate.Value > DateTime.UtcNow);
}