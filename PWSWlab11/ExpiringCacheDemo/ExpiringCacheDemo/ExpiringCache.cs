using System.Collections.Concurrent;
using System.Threading;
using System;

namespace ExpiringCacheDemo;

// Create a class that manages a thread-safe cache with expiration.
// The cache should support get, set, and remove operations.
// Items should automatically expire after a configurable TTL.
// Use ConcurrentDictionary for thread safety.

public class ExpiringCache<TKey, TValue> : IDisposable where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, CacheEntry> _cache = new();
    private readonly TimeSpan _defaultTtl;
    private readonly Timer _cleanupTimer;
    private int _cleanupRunning;
    private bool _disposed;

    // cleanupInterval: how often to scan for expired entries. If null, defaults to 1/4 of TTL (min 1s).
    public ExpiringCache(TimeSpan defaultTtl, TimeSpan? cleanupInterval = null)
    {
        _defaultTtl = defaultTtl;

        var interval = cleanupInterval ?? TimeSpan.FromMilliseconds(Math.Max(1000, defaultTtl.TotalMilliseconds / 4));
        _cleanupTimer = new Timer(_ => RemoveExpiredEntries(), null, interval, interval);
    }

    public void Set(TKey key, TValue value, TimeSpan? ttl = null)
    {
        ThrowIfDisposed();
        var expiry = DateTime.UtcNow + (ttl ?? _defaultTtl);
        _cache[key] = new CacheEntry(value, expiry);
    }

    // Returns value or default(TValue) if not present or expired
    public TValue? Get(TKey key)
    {
        if (TryGetValue(key, out var value))
            return value;

        return default;
    }

    // Try get pattern to avoid ambiguity with default(TValue)
    public bool TryGetValue(TKey key, out TValue? value)
    {
        ThrowIfDisposed();
        value = default;

        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.ExpiresAt > DateTime.UtcNow)
            {
                value = entry.Value;
                return true;
            }

            _cache.TryRemove(key, out _);
        }

        return false;
    }

    public bool Remove(TKey key)
    {
        ThrowIfDisposed();
        return _cache.TryRemove(key, out _);
    }

    public int Count => _cache.Count;

    public void Clear()
    {
        ThrowIfDisposed();
        _cache.Clear();
    }

    private void RemoveExpiredEntries()
    {
        // prevent overlapping runs
        if (Interlocked.Exchange(ref _cleanupRunning, 1) == 1)
            return;

        try
        {
            var now = DateTime.UtcNow;
            foreach (var kvp in _cache)
            {
                if (kvp.Value.ExpiresAt <= now)
                    _cache.TryRemove(kvp.Key, out _);
            }
        }
        catch
        {
            // suppress any exception from cleanup to keep timer alive
        }
        finally
        {
            Interlocked.Exchange(ref _cleanupRunning, 0);
        }
    }

    private record CacheEntry(TValue Value, DateTime ExpiresAt);

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ExpiringCache<TKey, TValue>));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _cleanupTimer.Dispose();
        _cache.Clear();
    }
}
