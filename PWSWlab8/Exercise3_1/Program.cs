using Exercise3_1.Models;
using Exercise3_1.Proxies;
using Exercise3_1.Repositories;

Console.WriteLine("=== Cwiczenie 3.1: Proxy dla repozytorium danych medycznych ===\n");

var patientId = Guid.Parse("11111111-1111-1111-1111-111111111111");

void TestRole(User user)
{
    Console.WriteLine($"\n--- Rola: {user.Role} ({user.Username}) ---");

    var lazyProxy = new LazyMedicalRecordProxy(() => new MedicalRecordRepository());
    IMedicalRecordRepository repo = new ProtectionProxy(lazyProxy, user);
    var loggingProxy = new LoggingProxy(repo, user);

    Console.WriteLine($"Lazy loaded przed wywolaniem: {lazyProxy.IsLoaded}");

    try
    {
        var records = loggingProxy.GetRecords(patientId);
        Console.WriteLine($"Lazy loaded po wywolaniu: {lazyProxy.IsLoaded}");
        Console.WriteLine($"Znaleziono {records.Count} rekordow");
        foreach (var r in records)
            Console.WriteLine($"  Dx={r.Diagnosis}, Leczenie={r.Treatment}, Notatki={r.DoctorNotes}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Blad: {ex.Message}");
    }
}

TestRole(new User("dr.kowalski", UserRole.Doctor));
TestRole(new User("pielegniarka.nowak", UserRole.Nurse));
TestRole(new User("audytor.szpital", UserRole.Auditor));
TestRole(new User("admin.system", UserRole.Administrator));

Console.WriteLine("\n=== Koniec cwiczenia 3.1 ===");
