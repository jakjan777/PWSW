use windows::core::{Result, HSTRING};
use windows::Win32::System::Registry::{
    RegCloseKey, RegOpenKeyExW, RegQueryValueExW, HKEY, HKEY_LOCAL_MACHINE, KEY_READ,
};

pub fn read_registry_string(key_path: &str, value_name: &str) -> Result<String> {
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

        let mut buffer = [0u16; 256];
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

pub fn read_windows_version() -> Result<()> {
    let name = read_registry_string(
        "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion",
        "ProductName",
    )?;
    println!("System: {name}");
    Ok(())
}
