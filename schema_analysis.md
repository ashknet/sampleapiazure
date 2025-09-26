# Entity Framework vs Database Schema Analysis

## Issues Found:

### 1. **Appointments Table**
**Entity Properties:**
- PatientId, ProviderId, OrganizationId, LocationId, DepartmentId
- StartDateTime, EndDateTime, AppointmentType, Status, Reason, Notes
- IsVirtual, VirtualMeetingUrl, CheckInTime, CheckOutTime, CancellationReason

**Database Schema Issues:**
- Missing: IsVirtual, VirtualMeetingUrl, CheckInTime, CheckOutTime, CancellationReason
- Column name mismatch: StartDateTime vs StartTime, EndDateTime vs EndTime

### 2. **AuditLogs Table**
**Entity Properties:**
- UserId, UserName, UserRole, Action, Resource, ResourceId
- PatientId, OrganizationId, Timestamp, IpAddress, UserAgent
- PurposeOfUse, ConsentId, Success, FailureReason, AdditionalData

**Database Schema Issues:**
- Missing: UserName, UserRole, Success, FailureReason, AdditionalData
- Column name mismatch: PatientId vs PatientId (string vs Guid), OrganizationId vs OrganizationId (string vs Guid)

### 3. **ConnectionLinks Table**
**Entity Properties:**
- ShareTokenId, AccessToken, IssuedAt, ExpiresAt, IpAddress, UserAgent, IsRevoked

**Database Schema Issues:**
- Missing: ShareTokenId, AccessToken, IssuedAt, ExpiresAt, IpAddress, UserAgent, IsRevoked
- This table seems to be completely different from the entity

### 4. **Consents Table**
**Entity Properties:**
- PatientId, OrganizationId, Type, Status, ConsentDate, ExpirationDate
- PurposeOfUse, ConsentingParty, RevokedBy, RevokedDate, ScopeJson

**Database Schema Issues:**
- Missing: ConsentingParty, RevokedBy, RevokedDate
- Column name mismatch: ConsentDate vs StartDate, ExpirationDate vs EndDate, ScopeJson vs Scopes

### 5. **ConsentEvents Table**
**Entity Properties:**
- ConsentId, EventType, Description, PerformedBy, OccurredAt

**Database Schema Issues:**
- Missing: Description, PerformedBy, OccurredAt
- Column name mismatch: EventType vs EventType (same), EventData vs Description

### 6. **Documents Table**
**Entity Properties:**
- PatientId, EncounterId, UploadedByUserId, OrganizationId, Title, DocumentType
- Category, FileName, ContentType, FileSizeBytes, BlobStorageUrl, Checksum
- DocumentDate, IsConfidential, ConfidentialityCode, Status, FhirDocumentReferenceId

**Database Schema Issues:**
- Missing: EncounterId, Category, FileSizeBytes, BlobStorageUrl, Checksum, IsConfidential, ConfidentialityCode, FhirDocumentReferenceId
- Column name mismatch: UploadedByUserId vs AuthorId, ContentType vs MimeType, BlobStorageUrl vs StorageUrl, FhirDocumentReferenceId vs FhirDocumentReference

### 7. **DocumentShares Table**
**Entity Properties:**
- DocumentId, SharedWithUserId, SharedByUserId, SharedAt, ExpiresAt
- Purpose, CanDownload, CanPrint, LastAccessedAt, AccessCount

**Database Schema Issues:**
- Missing: SharedByUserId, SharedAt, Purpose, CanDownload, CanPrint, LastAccessedAt, AccessCount
- Column name mismatch: SharedWithUserId vs SharedWithUserId (same)

### 8. **IntegrationEndpoints Table**
**Entity Properties:**
- OrganizationId, Name, Type, Direction, EndpointUrl, AuthType
- ConfigurationJson, IsActive, LastSuccessfulConnection, LastError, LastErrorAt

**Database Schema Issues:**
- Missing: Direction, LastSuccessfulConnection, LastError, LastErrorAt
- Column name mismatch: EndpointUrl vs Url, ConfigurationJson vs Configuration, LastSuccessfulConnection vs LastTestAt, LastError vs LastTestStatus

### 9. **NotificationPreferences Table**
**Entity Properties:**
- UserId, Channel, NotificationType, IsEnabled, Frequency, AdditionalSettings

**Database Schema Issues:**
- Missing: Channel, IsEnabled, Frequency, AdditionalSettings
- Column name mismatch: NotificationType vs NotificationType (same), IsEnabled vs EmailEnabled/SmsEnabled/PushEnabled

### 10. **Patients Table**
**Entity Properties:**
- UserId, MedicalRecordNumber, SocialSecurityNumber, Gender, PreferredLanguage
- Race, Ethnicity, MaritalStatus, Religion, IsDeceased, DeceasedDate

**Database Schema Issues:**
- Missing: SocialSecurityNumber, IsDeceased
- Column name mismatch: MedicalRecordNumber vs MRN

### 11. **Payers Table** (Already identified)
**Entity Properties:**
- Name, Code, PayerId, Type, ContactPhone, ContactEmail, Website, IsActive

**Database Schema Issues:**
- Column name mismatch: ContactPhone vs Phone, ContactEmail vs Email

### 12. **Plans Table**
**Entity Properties:**
- PayerId, Name, Code, Type, GroupNumber, RequiresReferral, RequiresPreAuthorization, IsActive

**Database Schema Issues:**
- Missing: RequiresReferral, RequiresPreAuthorization
- Column name mismatch: RequiresPreAuthorization vs RequiresAuthorization

### 13. **ShareTokens Table**
**Entity Properties:**
- PatientId, OrganizationId, Code, Type, Status, ExpiresAt
- RequestedScopes, AuthorizedScopes, AuthorizedAt, AuthorizedBy, LastAccessedAt, AccessCount

**Database Schema Issues:**
- Missing: PatientId, Type, AuthorizedScopes, AuthorizedAt, AuthorizedBy, LastAccessedAt, AccessCount
- Column name mismatch: RequestedScopes vs Scopes, AuthorizedBy vs AuthorizedByPatientId

### 14. **Users Table**
**Entity Properties:**
- ExternalId, Email, FirstName, LastName, MiddleName, UserType, PhoneNumber
- DateOfBirth, IsActive, LastLoginAt, NpiNumber, LicenseNumber, Specialty

**Database Schema Issues:**
- Missing: PhoneNumber, LastLoginAt
- Column name mismatch: PhoneNumber vs Phone, DateOfBirth vs DateOfBirth (same)

### 15. **UserOrganizationAssignments Table**
**Entity Properties:**
- UserId, OrganizationId, DepartmentId, StartDate, EndDate, IsPrimary

**Database Schema Issues:**
- Missing: DepartmentId
- All other columns match

### 16. **UserRoles Table**
**Entity Properties:**
- UserId, RoleId, OrganizationId, ExpiresAt

**Database Schema Issues:**
- Missing: ExpiresAt
- All other columns match

## Summary of Major Issues:
1. **Missing Columns**: Many tables are missing columns that exist in entities
2. **Column Name Mismatches**: Several properties have different column names in database
3. **Data Type Mismatches**: Some properties have different data types (string vs Guid)
4. **Missing Configuration Files**: Many entities don't have EF configuration files