use windows::core::{Result, HSTRING};
use windows::Win32::Graphics::Dxgi::{
    CreateDXGIFactory1, DXGI_ADAPTER_FLAG_SOFTWARE, IDXGIFactory1,
};
use windows::Win32::System::SystemInformation::{
    GlobalMemoryStatusEx, GetSystemInfo, MEMORYSTATUSEX, PROCESSOR_ARCHITECTURE_AMD64,
    PROCESSOR_ARCHITECTURE_ARM64, PROCESSOR_ARCHITECTURE_INTEL, SYSTEM_INFO,
};
use windows::Win32::System::Registry::{
    RegCloseKey, RegOpenKeyExW, RegQueryValueExW, HKEY, HKEY_LOCAL_MACHINE, KEY_READ,
};

fn architektura_procesora(info: &SYSTEM_INFO) -> &'static str {
    unsafe {
        match info.Anonymous.Anonymous.wProcessorArchitecture {
            PROCESSOR_ARCHITECTURE_AMD64 => "x86-64",
            PROCESSOR_ARCHITECTURE_ARM64 => "ARM64",
            PROCESSOR_ARCHITECTURE_INTEL => "x86 (32-bit)",
            _ => "Nieznana",
        }
    }
}

fn odczytaj_rejestr_string(key_path: &str, value_name: &str) -> Result<String> {
    unsafe {
        let mut key = HKEY::default();
        RegOpenKeyExW(
            HKEY_LOCAL_MACHINE,
            &HSTRING::from(key_path),
            0,
            KEY_READ,
            &mut key,
        )
        .ok()?;

        let mut buffer = [0u16; 512];
        let mut size = (buffer.len() * 2) as u32;
        RegQueryValueExW(
            key,
            &HSTRING::from(value_name),
            None,
            None,
            Some(buffer.as_mut_ptr().cast()),
            Some(&mut size),
        )
        .ok()?;

        RegCloseKey(key).ok()?;

        let len = (size as usize / 2).saturating_sub(1);
        Ok(String::from_utf16_lossy(&buffer[..len]))
    }
}

fn nazwa_procesora() -> String {
    odczytaj_rejestr_string(
        "HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0",
        "ProcessorNameString",
    )
    .unwrap_or_else(|_| "Nieznany procesor".into())
}

fn formatuj_pamiec_vram(bajty: u64) -> String {
    if bajty >= 1024 * 1024 * 1024 {
        format!("{} GB", bajty / (1024 * 1024 * 1024))
    } else {
        format!("{} MB", bajty / (1024 * 1024))
    }
}

fn wide_string_do_rust(wide: &[u16]) -> String {
    let end = wide.iter().position(|&c| c == 0).unwrap_or(wide.len());
    String::from_utf16_lossy(&wide[..end])
}

pub fn print_system_info() {
    unsafe {
        let mut info = SYSTEM_INFO::default();
        GetSystemInfo(&mut info);

        println!("--- Procesor ---");
        println!("Model: {}", nazwa_procesora().trim());
        println!("Architektura: {}", architektura_procesora(&info));
        println!("Rdzenie logiczne: {}", info.dwNumberOfProcessors);
        println!("Rozmiar strony pamieci: {} KB", info.dwPageSize / 1024);
        println!(
            "Poziom / rewizja CPU: {} / {}",
            info.wProcessorLevel, info.wProcessorRevision
        );

        let mut mem = MEMORYSTATUSEX {
            dwLength: std::mem::size_of::<MEMORYSTATUSEX>() as u32,
            ..Default::default()
        };
        GlobalMemoryStatusEx(&mut mem).expect("Blad odczytu pamieci");

        println!("\n--- Pamiec RAM ---");
        let gb = 1024 * 1024 * 1024;
        println!("Calkowita: {} GB", mem.ullTotalPhys / gb);
        println!("Dostepna: {} GB", mem.ullAvailPhys / gb);
        println!("Uzycie: {}%", mem.dwMemoryLoad);

        println!("\n--- Karty graficzne ---");
        match odczytaj_karty_graficzne() {
            Ok(karty) if karty.is_empty() => println!("Nie wykryto kart graficznych."),
            Ok(karty) => {
                for (i, karta) in karty.iter().enumerate() {
                    println!(
                        "  [{}] {} | VRAM: {} | Typ: {}",
                        i + 1,
                        karta.nazwa,
                        karta.vram,
                        karta.typ
                    );
                }
            }
            Err(err) => println!("Blad odczytu GPU: {err}"),
        }
    }
}

struct KartaGraficzna {
    nazwa: String,
    vram: String,
    typ: &'static str,
}

fn odczytaj_karty_graficzne() -> Result<Vec<KartaGraficzna>> {
    unsafe {
        let factory: IDXGIFactory1 = CreateDXGIFactory1()?;
        let mut karty = Vec::new();
        let mut index = 0u32;

        loop {
            let adapter = match factory.EnumAdapters1(index) {
                Ok(adapter) => adapter,
                Err(_) => break,
            };

            let desc = adapter.GetDesc1()?;
            let nazwa = wide_string_do_rust(&desc.Description);
            let vram = formatuj_pamiec_vram(desc.DedicatedVideoMemory as u64);
            let typ = if desc.Flags & (DXGI_ADAPTER_FLAG_SOFTWARE.0 as u32) != 0 {
                "programowa"
            } else {
                "sprzetowa"
            };

            if !nazwa.is_empty() {
                karty.push(KartaGraficzna { nazwa, vram, typ });
            }

            index += 1;
        }

        Ok(karty)
    }
}
