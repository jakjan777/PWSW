using Exercise3_1.Models;

namespace Exercise3_1.Repositories;

public class MedicalRecordRepository : IMedicalRecordRepository
{
    private readonly List<Patient> _patients = [];
    private readonly List<MedicalRecord> _records = [];

    public MedicalRecordRepository()
    {
        var patientId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        _patients.Add(new Patient(patientId, "Jan", "Kowalski", "90010112345"));
        _records.Add(new MedicalRecord(
            Guid.NewGuid(), patientId,
            "Zapalenie oskrzeli", "Antybiotyk 7 dni",
            "Pacjent palacz, monitorowac oddech",
            DateTime.UtcNow.AddDays(-2), "dr. Nowak"));
    }

    public Patient? GetPatient(Guid patientId) =>
        _patients.FirstOrDefault(p => p.Id == patientId);

    public List<MedicalRecord> GetRecords(Guid patientId) =>
        _records.Where(r => r.PatientId == patientId).ToList();

    public void AddRecord(MedicalRecord record) => _records.Add(record);
}
