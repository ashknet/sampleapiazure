using Hl7.Fhir.Model;

namespace Emr.Application.Common.Interfaces;

public interface IFhirService
{
    // Patient operations
    Task<Patient> GetPatientAsync(string patientId, CancellationToken cancellationToken = default);
    Task<Patient> CreatePatientAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<Patient> UpdatePatientAsync(Patient patient, CancellationToken cancellationToken = default);
    
    // Coverage operations
    Task<Coverage> GetCoverageAsync(string coverageId, CancellationToken cancellationToken = default);
    Task<Coverage> CreateCoverageAsync(Coverage coverage, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientCoveragesAsync(string patientId, CancellationToken cancellationToken = default);
    
    // Clinical resources
    Task<Bundle> GetPatientConditionsAsync(string patientId, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientAllergyIntolerancesAsync(string patientId, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientMedicationsAsync(string patientId, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientImmunizationsAsync(string patientId, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientObservationsAsync(string patientId, string? category = null, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientDiagnosticReportsAsync(string patientId, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientProceduresAsync(string patientId, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientEncountersAsync(string patientId, CancellationToken cancellationToken = default);
    
    // Document operations
    Task<DocumentReference> CreateDocumentReferenceAsync(DocumentReference documentReference, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientDocumentReferencesAsync(string patientId, CancellationToken cancellationToken = default);
    
    // Imaging operations
    Task<ImagingStudy> GetImagingStudyAsync(string studyId, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientImagingStudiesAsync(string patientId, CancellationToken cancellationToken = default);
    
    // Composition operations
    Task<Composition> CreateCompositionAsync(Composition composition, CancellationToken cancellationToken = default);
    Task<Bundle> GetPatientCompositionsAsync(string patientId, CancellationToken cancellationToken = default);
    
    // Helper methods
    Task<string> MapToFhirPatientIdAsync(Guid internalPatientId, CancellationToken cancellationToken = default);
    Task<Guid?> MapFromFhirPatientIdAsync(string fhirPatientId, CancellationToken cancellationToken = default);
}