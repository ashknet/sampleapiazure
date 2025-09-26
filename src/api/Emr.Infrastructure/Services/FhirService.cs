using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Emr.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace Emr.Infrastructure.Services;

public class FhirService : IFhirService
{
    private readonly FhirClient _fhirClient;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<FhirService> _logger;
    private readonly string _fhirServerUrl;

    public FhirService(
        IConfiguration configuration,
        IApplicationDbContext context,
        ILogger<FhirService> logger)
    {
        _context = context;
        _logger = logger;
        _fhirServerUrl = configuration["FhirServer:Url"] ?? throw new InvalidOperationException("FHIR server URL not configured");
        
        var settings = new FhirClientSettings
        {
            PreferredFormat = ResourceFormat.Json,
            ReturnPreference = ReturnPreference.Representation,
            Timeout = 30000, // 30 seconds in milliseconds
            VerifyFhirVersion = false
        };
        
        _fhirClient = new FhirClient(_fhirServerUrl, settings);
    }

    public async Task<Patient> GetPatientAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Read<Patient>($"Patient/{patientId}"), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient {PatientId} from FHIR server", patientId);
            throw;
        }
    }

    public async Task<Patient> CreatePatientAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        try
        {
            // Add provenance
            patient.Meta = new Meta
            {
                Profile = new[] { "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient" }
            };

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Create(patient), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient in FHIR server");
            throw;
        }
    }

    public async Task<Patient> UpdatePatientAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        try
        {
            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Update(patient), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating patient {PatientId} in FHIR server", patient.Id);
            throw;
        }
    }

    public async Task<Coverage> GetCoverageAsync(string coverageId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Read<Coverage>($"Coverage/{coverageId}"), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving coverage {CoverageId} from FHIR server", coverageId);
            throw;
        }
    }

    public async Task<Coverage> CreateCoverageAsync(Coverage coverage, CancellationToken cancellationToken = default)
    {
        try
        {
            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Create(coverage), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating coverage in FHIR server");
            throw;
        }
    }

    public async Task<Bundle> GetPatientCoveragesAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}")
                .Include("Coverage:payor");

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<Coverage>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving coverages for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<Bundle> GetPatientConditionsAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}")
                .OrderBy("recorded-date", SortOrder.Descending);

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<Condition>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conditions for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<Bundle> GetPatientAllergyIntolerancesAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}");

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<AllergyIntolerance>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving allergies for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<Bundle> GetPatientMedicationsAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}")
                .Include("MedicationRequest:medication");

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<MedicationRequest>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving medications for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<Bundle> GetPatientImmunizationsAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}")
                .OrderBy("date", SortOrder.Descending);

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<Immunization>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving immunizations for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<Bundle> GetPatientObservationsAsync(string patientId, string? category = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}");

            if (!string.IsNullOrEmpty(category))
            {
                searchParams = searchParams.Where($"category={category}");
            }

            searchParams = searchParams.OrderBy("date", SortOrder.Descending);

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<Observation>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving observations for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<Bundle> GetPatientDiagnosticReportsAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}")
                .Include("DiagnosticReport:result")
                .OrderBy("date", SortOrder.Descending);

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<DiagnosticReport>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving diagnostic reports for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<Bundle> GetPatientProceduresAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}")
                .OrderBy("date", SortOrder.Descending);

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<Procedure>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving procedures for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<Bundle> GetPatientEncountersAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}")
                .Include("Encounter:location")
                .Include("Encounter:participant")
                .OrderBy("date", SortOrder.Descending);

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<Encounter>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving encounters for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<DocumentReference> CreateDocumentReferenceAsync(DocumentReference documentReference, CancellationToken cancellationToken = default)
    {
        try
        {
            // Add US Core profile
            documentReference.Meta = new Meta
            {
                Profile = new[] { "http://hl7.org/fhir/us/core/StructureDefinition/us-core-documentreference" }
            };

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Create(documentReference), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document reference in FHIR server");
            throw;
        }
    }

    public async Task<Bundle> GetPatientDocumentReferencesAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}")
                .OrderBy("date", SortOrder.Descending);

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<DocumentReference>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document references for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<ImagingStudy> GetImagingStudyAsync(string studyId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Read<ImagingStudy>($"ImagingStudy/{studyId}"), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving imaging study {StudyId} from FHIR server", studyId);
            throw;
        }
    }

    public async Task<Bundle> GetPatientImagingStudiesAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}")
                .OrderBy("started", SortOrder.Descending);

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<ImagingStudy>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving imaging studies for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<Composition> CreateCompositionAsync(Composition composition, CancellationToken cancellationToken = default)
    {
        try
        {
            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Create(composition), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating composition in FHIR server");
            throw;
        }
    }

    public async Task<Bundle> GetPatientCompositionsAsync(string patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParams = new SearchParams()
                .Where($"patient=Patient/{patientId}")
                .Include("Composition:entry")
                .OrderBy("date", SortOrder.Descending);

            return await System.Threading.Tasks.Task.Run(() => _fhirClient.Search<Composition>(searchParams), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving compositions for patient {PatientId}", patientId);
            throw;
        }
    }

    public async Task<string> MapToFhirPatientIdAsync(Guid internalPatientId, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .Where(p => p.Id == internalPatientId)
            .Select(p => new { p.MedicalRecordNumber })
            .FirstOrDefaultAsync(cancellationToken);

        if (patient == null)
        {
            throw new InvalidOperationException($"Patient with ID {internalPatientId} not found");
        }

        // For this implementation, we'll use MRN as the FHIR patient ID
        // In production, you might have a separate mapping table
        return patient.MedicalRecordNumber;
    }

    public async Task<Guid?> MapFromFhirPatientIdAsync(string fhirPatientId, CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .Where(p => p.MedicalRecordNumber == fhirPatientId)
            .Select(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return patient == Guid.Empty ? null : patient;
    }
}