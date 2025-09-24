using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Guid? ProviderId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? LocationId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public DateTime StartDateTime { get; private set; }
    public DateTime EndDateTime { get; private set; }
    public string AppointmentType { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty; // e.g., "Scheduled", "CheckedIn", "InProgress", "Completed", "Cancelled", "NoShow"
    public string? Reason { get; private set; }
    public string? Notes { get; private set; }
    public bool IsVirtual { get; private set; }
    public string? VirtualMeetingUrl { get; private set; }
    public DateTime? CheckInTime { get; private set; }
    public DateTime? CheckOutTime { get; private set; }
    public string? CancellationReason { get; private set; }
    
    // Navigation properties
    public virtual Patient Patient { get; private set; } = null!;
    public virtual User? Provider { get; private set; }
    public virtual Organization Organization { get; private set; } = null!;
    public virtual Location? Location { get; private set; }
    public virtual Department? Department { get; private set; }
    
    protected Appointment() { }
    
    public Appointment(
        Guid patientId,
        Guid organizationId,
        DateTime startDateTime,
        DateTime endDateTime,
        string appointmentType,
        Guid? providerId = null,
        Guid? locationId = null,
        Guid? departmentId = null)
    {
        PatientId = patientId;
        OrganizationId = organizationId;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        AppointmentType = appointmentType;
        ProviderId = providerId;
        LocationId = locationId;
        DepartmentId = departmentId;
        Status = "Scheduled";
    }
    
    public void UpdateDetails(string? reason, string? notes)
    {
        Reason = reason;
        Notes = notes;
    }
    
    public void SetAsVirtual(string virtualMeetingUrl)
    {
        IsVirtual = true;
        VirtualMeetingUrl = virtualMeetingUrl;
    }
    
    public void CheckIn()
    {
        Status = "CheckedIn";
        CheckInTime = DateTime.UtcNow;
    }
    
    public void StartAppointment()
    {
        Status = "InProgress";
    }
    
    public void Complete()
    {
        Status = "Completed";
        CheckOutTime = DateTime.UtcNow;
    }
    
    public void Cancel(string cancellationReason)
    {
        Status = "Cancelled";
        CancellationReason = cancellationReason;
    }
    
    public void MarkAsNoShow()
    {
        Status = "NoShow";
    }
    
    public void Reschedule(DateTime newStartDateTime, DateTime newEndDateTime)
    {
        StartDateTime = newStartDateTime;
        EndDateTime = newEndDateTime;
    }
}