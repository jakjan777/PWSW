using Exercise3_1.Models;
using Exercise3_1.Repositories;

namespace Exercise3_1.Proxies;

public record AuditEntry(
    DateTime Timestamp,
    string Username,
    string Operation,
    string Details,
    bool Success);

public class LoggingProxy(IMedicalRecordRepository inner, User user)
    : IMedicalRecordRepository
{
    private readonly List<AuditEntry> _log = [];
    public IReadOnlyList<AuditEntry> AuditLog => _log.AsReadOnly();

    public Patient? GetPatient(Guid patientId)
    {
        try
        {
            var result = inner.GetPatient(patientId);
            Log("GetPatient", $"Id={patientId}, Found={result is not null}");
            return result;
        }
        catch (Exception ex)
        {
            Log("GetPatient", $"BLAD: {ex.Message}", false);
            throw;
        }
    }

    public List<MedicalRecord> GetRecords(Guid patientId)
    {
        try
        {
            var result = inner.GetRecords(patientId);
            Log("GetRecords", $"Id={patientId}, Count={result.Count}");
            return result;
        }
        catch (Exception ex)
        {
            Log("GetRecords", $"BLAD: {ex.Message}", false);
            throw;
        }
    }

    public void AddRecord(MedicalRecord record)
    {
        try
        {
            inner.AddRecord(record);
            Log("AddRecord", $"PatientId={record.PatientId}, Dx={record.Diagnosis}");
        }
        catch (Exception ex)
        {
            Log("AddRecord", $"BLAD: {ex.Message}", false);
            throw;
        }
    }

    private void Log(string op, string details, bool ok = true)
    {
        _log.Add(new(DateTime.UtcNow, user.Username, op, details, ok));
        Console.WriteLine(
            $"[AUDIT] [{(ok ? "OK" : "ERR")}] {user.Username} -> {op}: {details}");
    }
}
