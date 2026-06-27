using Exercise3_1.Models;
using Exercise3_1.Repositories;

namespace Exercise3_1.Proxies;

public class ProtectionProxy(IMedicalRecordRepository inner, User user)
    : IMedicalRecordRepository
{
    public Patient? GetPatient(Guid patientId)
    {
        if (user.Role == UserRole.Administrator)
            throw new UnauthorizedAccessException(
                $"Rola '{user.Role}' nie ma dostepu do danych pacjentow.");

        return inner.GetPatient(patientId);
    }

    public List<MedicalRecord> GetRecords(Guid patientId) =>
        user.Role switch
        {
            UserRole.Doctor => inner.GetRecords(patientId),
            UserRole.Nurse => inner.GetRecords(patientId)
                .Select(r => r with { DoctorNotes = "[BRAK DOSTEPU]" })
                .ToList(),
            UserRole.Auditor => inner.GetRecords(patientId)
                .Select(r => r with
                {
                    Diagnosis = "[ZANONIMIZOWANE]",
                    Treatment = "[ZANONIMIZOWANE]",
                    DoctorNotes = "[ZANONIMIZOWANE]"
                })
                .ToList(),
            _ => throw new UnauthorizedAccessException(
                $"Rola '{user.Role}' nie ma dostepu do rekordow.")
        };

    public void AddRecord(MedicalRecord record)
    {
        if (user.Role != UserRole.Doctor)
            throw new UnauthorizedAccessException(
                "Tylko lekarz moze dodawac rekordy medyczne.");

        inner.AddRecord(record);
    }
}
