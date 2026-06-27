using Exercise3_1.Models;
using Exercise3_1.Repositories;

namespace Exercise3_1.Proxies;

public class LazyMedicalRecordProxy(Func<IMedicalRecordRepository> factory)
    : IMedicalRecordRepository
{
    private readonly Lazy<IMedicalRecordRepository> _lazy =
        new(factory, LazyThreadSafetyMode.ExecutionAndPublication);

    private IMedicalRecordRepository Real => _lazy.Value;
    public bool IsLoaded => _lazy.IsValueCreated;

    public Patient? GetPatient(Guid id) => Real.GetPatient(id);
    public List<MedicalRecord> GetRecords(Guid id) => Real.GetRecords(id);
    public void AddRecord(MedicalRecord r) => Real.AddRecord(r);
}
