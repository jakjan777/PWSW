using Exercise3_1.Models;

namespace Exercise3_1.Repositories;

public interface IMedicalRecordRepository
{
    Patient? GetPatient(Guid patientId);
    List<MedicalRecord> GetRecords(Guid patientId);
    void AddRecord(MedicalRecord record);
}
