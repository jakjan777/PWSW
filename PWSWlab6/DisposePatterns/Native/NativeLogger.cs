namespace DisposePatterns.Native;

public static class NativeLogger
{
    private static int _nextHandle = 1;
    private static readonly HashSet<int> _openHandles = [];

    public static IntPtr Open(string name)
    {
        int handle = _nextHandle++;
        _openHandles.Add(handle);
        Console.WriteLine($"  [NativeLogger] Otwarto uchwyt {handle} dla {name}");
        return new IntPtr(handle);
    }

    public static void Close(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return;

        int id = handle.ToInt32();
        if (_openHandles.Remove(id))
            Console.WriteLine($"  [NativeLogger] Zamknieto uchwyt {id}");
    }

    public static bool IsOpen(IntPtr handle) =>
        handle != IntPtr.Zero && _openHandles.Contains(handle.ToInt32());
}
