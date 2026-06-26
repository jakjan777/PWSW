use windows::Win32::System::SystemInformation::{
    GlobalMemoryStatusEx, GetSystemInfo, MEMORYSTATUSEX, SYSTEM_INFO,
};

struct RawSystemSnapshot {
    logical_processors: u32,
    available_phys_bytes: u64,
}

fn read_system_snapshot_unsafe() -> RawSystemSnapshot {
    unsafe {
        let mut info = SYSTEM_INFO::default();
        GetSystemInfo(&mut info);

        let mut mem = MEMORYSTATUSEX {
            dwLength: std::mem::size_of::<MEMORYSTATUSEX>() as u32,
            ..Default::default()
        };
        GlobalMemoryStatusEx(&mut mem).expect("Blad odczytu pamieci");

        RawSystemSnapshot {
            logical_processors: info.dwNumberOfProcessors,
            available_phys_bytes: mem.ullAvailPhys,
        }
    }
}

pub fn get_cpu_count() -> u32 {
    read_system_snapshot_unsafe().logical_processors
}

pub fn get_available_memory_gb() -> u64 {
    let bytes = read_system_snapshot_unsafe().available_phys_bytes;
    bytes / (1024 * 1024 * 1024)
}
