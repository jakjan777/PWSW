namespace SecurityScanner;

public static class FullScanRunner
{
    public static async Task<int> RunFullScanAsync(
        int totalChecks,
        IProgress<ScanProgress> progress,
        CancellationToken ct = default)
    {
        int processed = 0;
        int batch = totalChecks / 10;

        for (int i = 0; i < totalChecks; i++)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(2, ct);
            processed++;

            if (processed % batch == 0)
            {
                int pct = 100 * processed / totalChecks;
                progress.Report(new ScanProgress(
                    pct,
                    $"Partia {processed / batch}/10",
                    processed));
            }
        }

        return processed;
    }
}
