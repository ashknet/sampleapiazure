using Microsoft.EntityFrameworkCore;
using Emr.Domain.Entities;
using Emr.Domain.ValueObjects;

namespace Emr.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static async Task SeedSampleDataAsync(ApplicationDbContext context)
    {
        // Seed Roles
        if (!await context.Roles.AnyAsync())
        {
            var roles = new[]
            {
                new Role(Role.Patient, "Patient", true),
                new Role(Role.Clinician, "Clinician", true),
                new Role(Role.Registrar, "Registrar", true),
                new Role(Role.HIM, "Health Information Management", true),
                new Role(Role.OrgAdmin, "Organization Administrator", true),
                new Role(Role.Integration, "Integration Service", true),
                new Role(Role.Auditor, "Auditor", true)
            };

            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();
        }

        // Seed Permissions
        if (!await context.Permissions.AnyAsync())
        {
            var permissions = new[]
            {
                // Patient permissions
                new Permission("Patient", "Read", "Own"),
                new Permission("Document", "Read", "Own"),
                new Permission("Document", "Write", "Own"),
                new Permission("Appointment", "Read", "Own"),
                new Permission("Consent", "Read", "Own"),
                new Permission("Consent", "Write", "Own"),
                
                // Clinician permissions
                new Permission("Patient", "Read", "Organization"),
                new Permission("Patient", "Write", "Organization"),
                new Permission("Document", "Read", "Organization"),
                new Permission("Document", "Write", "Organization"),
                new Permission("Appointment", "Read", "Organization"),
                new Permission("Appointment", "Write", "Organization"),
                
                // Admin permissions
                new Permission("User", "Read", "Organization"),
                new Permission("User", "Write", "Organization"),
                new Permission("Organization", "Read", "Own"),
                new Permission("Organization", "Write", "Own"),
                
                // Audit permissions
                new Permission("AuditLog", "Read", "Organization")
            };

            context.Permissions.AddRange(permissions);
            await context.SaveChangesAsync();
        }

        // Seed Organizations
        if (!await context.Organizations.AnyAsync())
        {
            var org1 = new Organization("General Hospital", "12-3456789", "Hospital");
            org1.UpdateDetails("General Hospital", "1234567890", "info@generalhospital.com", "555-0100", "www.generalhospital.com");
            
            var org2 = new Organization("City Medical Clinic", "98-7654321", "Clinic");
            org2.UpdateDetails("City Medical Clinic", "0987654321", "contact@citymedical.com", "555-0200", "www.citymedical.com");

            context.Organizations.AddRange(org1, org2);
            await context.SaveChangesAsync();

            // Add Locations
            var location1 = new Location(
                org1.Id,
                "Main Campus",
                "MAIN",
                new Address("123 Hospital Way", null, "Healthcare City", "HC", "12345", "USA"));
            location1.UpdateDetails("Main Campus", "555-0101", "555-0199");

            var location2 = new Location(
                org1.Id,
                "Emergency Department",
                "ED",
                new Address("123 Hospital Way", "Emergency Entrance", "Healthcare City", "HC", "12345", "USA"));
            location2.UpdateDetails("Emergency Department", "555-0111", null);

            context.Locations.AddRange(location1, location2);
            await context.SaveChangesAsync();

            // Add Departments
            var dept1 = new Department(org1.Id, "Cardiology", "CARD", location1.Id);
            dept1.UpdateDetails("Cardiology", "Cardiology");

            var dept2 = new Department(org1.Id, "Emergency Medicine", "EMER", location2.Id);
            dept2.UpdateDetails("Emergency Medicine", "Emergency Medicine");

            context.Departments.AddRange(dept1, dept2);
            await context.SaveChangesAsync();
        }

        // Seed Payers and Plans
        if (!await context.Payers.AnyAsync())
        {
            var payer1 = new Payer("Blue Cross Blue Shield", "BCBS", "Commercial");
            payer1.UpdateDetails("Blue Cross Blue Shield", "BCBS001", "800-555-0300", "info@bcbs.com", "www.bcbs.com");

            var payer2 = new Payer("Medicare", "MCARE", "Government");
            payer2.UpdateDetails("Medicare", "MCARE", "800-MEDICARE", "info@medicare.gov", "www.medicare.gov");

            context.Payers.AddRange(payer1, payer2);
            await context.SaveChangesAsync();

            var plan1 = new Plan(payer1.Id, "BCBS Gold", "GOLD", "PPO");
            plan1.UpdateDetails("BCBS Gold PPO", "GRP001", false, true);

            var plan2 = new Plan(payer1.Id, "BCBS Silver", "SILVER", "HMO");
            plan2.UpdateDetails("BCBS Silver HMO", "GRP002", true, true);

            var plan3 = new Plan(payer2.Id, "Medicare Part A", "PARTA", "Government");
            plan3.UpdateDetails("Medicare Part A", null, false, false);

            context.Plans.AddRange(plan1, plan2, plan3);
            await context.SaveChangesAsync();
        }

        // Note: In production, user data would come from Azure AD B2C
        // This is just for development/testing purposes
        if (!await context.Users.AnyAsync())
        {
            // Create test users
            var patientUser = new User("test-patient-001", "patient@example.com", "John", "Doe", "Patient");
            patientUser.UpdatePersonalInfo("John", "Doe", "Michael", "555-1234", new DateTime(1980, 1, 1));

            var clinicianUser = new User("test-clinician-001", "doctor@example.com", "Jane", "Smith", "Staff");
            clinicianUser.UpdatePersonalInfo("Jane", "Smith", null, "555-2345", new DateTime(1975, 6, 15));
            clinicianUser.UpdateStaffInfo("1234567890", "MD12345", "Cardiology");

            var registrarUser = new User("test-registrar-001", "registrar@example.com", "Bob", "Johnson", "Staff");
            registrarUser.UpdatePersonalInfo("Bob", "Johnson", null, "555-3456", null);

            context.Users.AddRange(patientUser, clinicianUser, registrarUser);
            await context.SaveChangesAsync();

            // Create patient profile
            var patient = new Patient(patientUser.Id, "MRN-001", "Male");
            patient.UpdateDemographics("en", "White", "Not Hispanic or Latino", "Married", null);
            patient.AddAddress(
                new Address("456 Patient Street", "Apt 2B", "Healthcare City", "HC", "12346", "USA"),
                "Home",
                true);
            patient.AddContact("Mary", "Doe", "Spouse", "555-1235", "mary.doe@example.com");

            context.Patients.Add(patient);
            await context.SaveChangesAsync();

            // Assign roles
            var patientRole = await context.Roles.FirstAsync(r => r.Name == Role.Patient);
            var clinicianRole = await context.Roles.FirstAsync(r => r.Name == Role.Clinician);
            var registrarRole = await context.Roles.FirstAsync(r => r.Name == Role.Registrar);

            var org = await context.Organizations.FirstAsync();

            context.UserRoles.AddRange(
                new UserRole(patientUser.Id, patientRole.Id),
                new UserRole(clinicianUser.Id, clinicianRole.Id, org.Id),
                new UserRole(registrarUser.Id, registrarRole.Id, org.Id)
            );

            context.UserOrganizationAssignments.AddRange(
                new UserOrganizationAssignment(clinicianUser.Id, org.Id, DateTime.UtcNow, true),
                new UserOrganizationAssignment(registrarUser.Id, org.Id, DateTime.UtcNow, true)
            );

            await context.SaveChangesAsync();
        }
    }
}