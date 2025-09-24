using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System.Text.Json;

namespace Emr.Integration
{
    public class HL7MessageProcessor
    {
        private readonly ILogger<HL7MessageProcessor> _logger;
        private readonly FhirClient _fhirClient;

        public HL7MessageProcessor(ILogger<HL7MessageProcessor> logger, FhirClient fhirClient)
        {
            _logger = logger;
            _fhirClient = fhirClient;
        }

        [Function("ProcessHL7Message")]
        public async Task ProcessHL7Message(
            [ServiceBusTrigger("hl7-messages", Connection = "ServiceBusConnection")] 
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            try
            {
                _logger.LogInformation($"Processing HL7 message: {message.MessageId}");
                
                var hl7Content = message.Body.ToString();
                var messageType = message.ApplicationProperties["MessageType"]?.ToString();

                switch (messageType)
                {
                    case "ADT^A01": // Admission
                        await ProcessAdmission(hl7Content);
                        break;
                    case "ADT^A08": // Update patient information
                        await ProcessPatientUpdate(hl7Content);
                        break;
                    case "ORU^R01": // Observation result
                        await ProcessObservationResult(hl7Content);
                        break;
                    default:
                        _logger.LogWarning($"Unsupported message type: {messageType}");
                        break;
                }

                // Complete the message
                await messageActions.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing HL7 message: {message.MessageId}");
                
                // Move to dead letter queue after retries
                if (message.DeliveryCount >= 3)
                {
                    await messageActions.DeadLetterMessageAsync(message, "ProcessingFailed", ex.Message);
                }
                else
                {
                    // Abandon for retry
                    await messageActions.AbandonMessageAsync(message);
                }
            }
        }

        private async Task ProcessAdmission(string hl7Content)
        {
            // Parse HL7 message (simplified - use actual HL7 parser library)
            var patientData = ParsePatientFromHL7(hl7Content);
            
            // Create FHIR Patient resource
            var patient = new Patient
            {
                Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        System = "http://hospital.example.org/mrn",
                        Value = patientData.MRN
                    }
                },
                Name = new List<HumanName>
                {
                    new HumanName
                    {
                        Given = new[] { patientData.FirstName },
                        Family = patientData.LastName
                    }
                },
                BirthDate = patientData.DateOfBirth.ToString("yyyy-MM-dd"),
                Gender = patientData.Gender == "M" ? AdministrativeGender.Male : 
                         patientData.Gender == "F" ? AdministrativeGender.Female : 
                         AdministrativeGender.Other
            };

            // Create or update patient in FHIR server
            var result = await _fhirClient.CreateAsync(patient);
            _logger.LogInformation($"Created FHIR patient: {result.Id}");
            
            // Create Encounter resource
            var encounter = new Encounter
            {
                Status = Encounter.EncounterStatus.InProgress,
                Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "IMP", "inpatient encounter"),
                Subject = new ResourceReference($"Patient/{result.Id}"),
                Period = new Period
                {
                    Start = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
                }
            };
            
            await _fhirClient.CreateAsync(encounter);
        }

        private async Task ProcessPatientUpdate(string hl7Content)
        {
            // Implementation for patient updates
            _logger.LogInformation("Processing patient update");
        }

        private async Task ProcessObservationResult(string hl7Content)
        {
            // Parse observation data from HL7
            var observationData = ParseObservationFromHL7(hl7Content);
            
            // Create FHIR Observation resource
            var observation = new Observation
            {
                Status = ObservationStatus.Final,
                Code = new CodeableConcept
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = observationData.LoincCode,
                            Display = observationData.TestName
                        }
                    }
                },
                Subject = new ResourceReference($"Patient/{observationData.PatientId}"),
                Effective = new FhirDateTime(observationData.ObservationDate),
                Value = new Quantity
                {
                    Value = observationData.Value,
                    Unit = observationData.Unit,
                    System = "http://unitsofmeasure.org"
                }
            };
            
            await _fhirClient.CreateAsync(observation);
            _logger.LogInformation($"Created FHIR observation for patient: {observationData.PatientId}");
        }

        private PatientData ParsePatientFromHL7(string hl7Content)
        {
            // Simplified parsing - use actual HL7 parser
            return new PatientData
            {
                MRN = "12345",
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = DateTime.Parse("1980-01-01"),
                Gender = "M"
            };
        }

        private ObservationData ParseObservationFromHL7(string hl7Content)
        {
            // Simplified parsing - use actual HL7 parser
            return new ObservationData
            {
                PatientId = "patient-123",
                LoincCode = "2951-2",
                TestName = "Sodium",
                Value = 140,
                Unit = "mmol/L",
                ObservationDate = DateTime.UtcNow
            };
        }
    }

    public class PatientData
    {
        public string MRN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
    }

    public class ObservationData
    {
        public string PatientId { get; set; }
        public string LoincCode { get; set; }
        public string TestName { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; }
        public DateTime ObservationDate { get; set; }
    }
}