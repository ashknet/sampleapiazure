using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class Document : BaseEntity
{
    public Guid? PatientId { get; private set; }
    public Guid? EncounterId { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string DocumentType { get; private set; } = string.Empty; // e.g., "ClinicalNote", "LabResult", "Imaging", "Consent"
    public string? Category { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public string BlobStorageUrl { get; private set; } = string.Empty;
    public string? Checksum { get; private set; }
    public DateTime DocumentDate { get; private set; }
    public bool IsConfidential { get; private set; }
    public string? ConfidentialityCode { get; private set; } // e.g., "42CFRPart2"
    public string Status { get; private set; } = string.Empty; // e.g., "Draft", "Final", "Amended"
    public string? FhirDocumentReferenceId { get; private set; }
    
    // Navigation properties
    public virtual Patient? Patient { get; private set; }
    public virtual User UploadedByUser { get; private set; } = null!;
    public virtual Organization Organization { get; private set; } = null!;
    public virtual ICollection<DocumentShare> DocumentShares { get; private set; } = new List<DocumentShare>();
    
    protected Document() { }
    
    public Document(
        Guid uploadedByUserId,
        Guid organizationId,
        string title,
        string documentType,
        string fileName,
        string contentType,
        long fileSizeBytes,
        string blobStorageUrl,
        DateTime documentDate,
        Guid? patientId = null)
    {
        UploadedByUserId = uploadedByUserId;
        OrganizationId = organizationId;
        Title = title;
        DocumentType = documentType;
        FileName = fileName;
        ContentType = contentType;
        FileSizeBytes = fileSizeBytes;
        BlobStorageUrl = blobStorageUrl;
        DocumentDate = documentDate;
        PatientId = patientId;
        Status = "Draft";
    }
    
    public void AssignToPatient(Guid patientId)
    {
        PatientId = patientId;
    }
    
    public void AssignToEncounter(Guid encounterId)
    {
        EncounterId = encounterId;
    }
    
    public void SetChecksum(string checksum)
    {
        Checksum = checksum;
    }
    
    public void MarkAsConfidential(string confidentialityCode)
    {
        IsConfidential = true;
        ConfidentialityCode = confidentialityCode;
    }
    
    public void FinalizeDocument()
    {
        Status = "Final";
    }
    
    public void Amend()
    {
        Status = "Amended";
    }
    
    public void SetFhirReference(string fhirDocumentReferenceId)
    {
        FhirDocumentReferenceId = fhirDocumentReferenceId;
    }
}