using Emr.Domain.Common;

namespace Emr.Domain.Entities;

public class DocumentShare : BaseEntity
{
    public Guid DocumentId { get; private set; }
    public Guid SharedWithUserId { get; private set; }
    public Guid SharedByUserId { get; private set; }
    public DateTime SharedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? Purpose { get; private set; }
    public bool CanDownload { get; private set; }
    public bool CanPrint { get; private set; }
    public DateTime? LastAccessedAt { get; private set; }
    public int AccessCount { get; private set; }
    
    // Navigation properties
    public virtual Document Document { get; private set; } = null!;
    public virtual User SharedWithUser { get; private set; } = null!;
    public virtual User SharedByUser { get; private set; } = null!;
    
    protected DocumentShare() { }
    
    public DocumentShare(
        Guid documentId,
        Guid sharedWithUserId,
        Guid sharedByUserId,
        bool canDownload = true,
        bool canPrint = true,
        DateTime? expiresAt = null)
    {
        DocumentId = documentId;
        SharedWithUserId = sharedWithUserId;
        SharedByUserId = sharedByUserId;
        SharedAt = DateTime.UtcNow;
        CanDownload = canDownload;
        CanPrint = canPrint;
        ExpiresAt = expiresAt;
    }
    
    public void RecordAccess()
    {
        LastAccessedAt = DateTime.UtcNow;
        AccessCount++;
    }
    
    public bool IsValid => !ExpiresAt.HasValue || ExpiresAt.Value > DateTime.UtcNow;
}