using Emr.Domain.Common;
using Emr.Domain.ValueObjects;

namespace Emr.Domain.Entities;

public class Patient : BaseEntity
{
    public Guid UserId { get; private set; }
    public string MedicalRecordNumber { get; private set; } = string.Empty;
    public string? SocialSecurityNumber { get; private set; }
    public string Gender { get; private set; } = string.Empty;
    public string? PreferredLanguage { get; private set; }
    public string? Race { get; private set; }
    public string? Ethnicity { get; private set; }
    public string? MaritalStatus { get; private set; }
    public string? Religion { get; private set; }
    public bool IsDeceased { get; private set; }
    public DateTime? DeceasedDate { get; private set; }
    
    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual ICollection<PatientIdentifier> Identifiers { get; private set; } = new List<PatientIdentifier>();
    public virtual ICollection<PatientContact> Contacts { get; private set; } = new List<PatientContact>();
    public virtual ICollection<PatientAddress> Addresses { get; private set; } = new List<PatientAddress>();
    public virtual ICollection<Coverage> Coverages { get; private set; } = new List<Coverage>();
    public virtual ICollection<Consent> Consents { get; private set; } = new List<Consent>();
    public virtual ICollection<Document> Documents { get; private set; } = new List<Document>();
    public virtual ICollection<ShareToken> ShareTokens { get; private set; } = new List<ShareToken>();
    public virtual ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();
    
    protected Patient() { }
    
    public Patient(Guid userId, string medicalRecordNumber, string gender)
    {
        UserId = userId;
        MedicalRecordNumber = medicalRecordNumber;
        Gender = gender;
    }
    
    public void UpdateDemographics(string? preferredLanguage, string? race, string? ethnicity, string? maritalStatus, string? religion)
    {
        PreferredLanguage = preferredLanguage;
        Race = race;
        Ethnicity = ethnicity;
        MaritalStatus = maritalStatus;
        Religion = religion;
    }
    
    public void UpdateSocialSecurityNumber(string? ssn)
    {
        SocialSecurityNumber = ssn;
    }
    
    public void MarkAsDeceased(DateTime deceasedDate)
    {
        IsDeceased = true;
        DeceasedDate = deceasedDate;
    }
    
    public void AddIdentifier(string system, string value, string? type = null)
    {
        var identifier = new PatientIdentifier(Id, system, value, type);
        Identifiers.Add(identifier);
    }
    
    public void AddContact(string firstName, string lastName, string relationship, string? phone, string? email)
    {
        var contact = new PatientContact(Id, firstName, lastName, relationship, phone, email);
        Contacts.Add(contact);
    }
    
    public void AddAddress(Address address, string type, bool isPrimary)
    {
        // If setting as primary, unset any existing primary
        if (isPrimary)
        {
            foreach (var addr in Addresses.Where(a => a.IsPrimary))
            {
                addr.UnsetPrimary();
            }
        }
        
        var patientAddress = new PatientAddress(Id, address, type, isPrimary);
        Addresses.Add(patientAddress);
    }
}