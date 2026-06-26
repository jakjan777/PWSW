use windows::core::{Result, HSTRING};
use windows::Win32::Storage::FileSystem::{
    CreateFileW, WriteFile, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, FILE_GENERIC_WRITE,
    FILE_SHARE_NONE,
};
use windows::Win32::System::SystemInformation::GetLocalTime;

pub fn safe_file_operations() -> Result<()> {
    let sciezka = "audit_report.txt";

    let handle = unsafe {
        CreateFileW(
            &HSTRING::from(sciezka),
            FILE_GENERIC_WRITE.0,
            FILE_SHARE_NONE,
            None,
            CREATE_ALWAYS,
            FILE_ATTRIBUTE_NORMAL,
            None,
        )?
    };

    let data = format!("Raport audytu systemowego\r\nData: {}\r\n", dzisiejsza_data());
    let mut written = 0u32;

    unsafe {
        WriteFile(
            handle,
            Some(data.as_bytes()),
            Some(&mut written),
            None,
        )?;
    }

    println!("Zapisano {written} bajtow do {sciezka}");
    // HANDLE zamyka sie automatycznie po Drop na koncu zakresu
    Ok(())
}

fn dzisiejsza_data() -> String {
    unsafe {
        let data = GetLocalTime();
        format!(
            "{:04}-{:02}-{:02}",
            data.wYear, data.wMonth, data.wDay
        )
    }
}
