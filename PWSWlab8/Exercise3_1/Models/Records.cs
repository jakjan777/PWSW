namespace Exercise3_1.Models;

public record Patient(Guid Id, string FirstName, string LastName, string Pesel);

public record MedicalRecord(
    Guid Id,
    Guid PatientId,
    string Diagnosis,
    string Treatment,
    string DoctorNotes,
    DateTime RecordDate,
    string DoctorName);

public record User(string Username, UserRole Role);

public enum UserRole
{
    Doctor,
    Nurse,
    Administrator,
    Auditor
}
