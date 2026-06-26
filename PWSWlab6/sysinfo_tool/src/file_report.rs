use windows::core::{Result, HSTRING};
use windows::Win32::Foundation::{CloseHandle, HANDLE};
use windows::Win32::Storage::FileSystem::{
    CreateFileW, WriteFile, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, FILE_GENERIC_WRITE,
    FILE_SHARE_NONE,
};

struct FileHandle(HANDLE);

impl FileHandle {
    fn create(path: &str) -> Result<Self> {
        unsafe {
            let handle = CreateFileW(
                &HSTRING::from(path),
                FILE_GENERIC_WRITE.0,
                FILE_SHARE_NONE,
                None,
                CREATE_ALWAYS,
                FILE_ATTRIBUTE_NORMAL,
                None,
            )?;
            Ok(Self(handle))
        }
    }

    fn write_all(&self, data: &[u8]) -> Result<u32> {
        unsafe {
            let mut written = 0u32;
            WriteFile(self.0, Some(data), Some(&mut written), None)?;
            Ok(written)
        }
    }
}

impl Drop for FileHandle {
    fn drop(&mut self) {
        unsafe {
            let _ = CloseHandle(self.0);
        }
    }
}

pub fn write_report_to_file(path: &str, content: &str) -> Result<()> {
    let handle = FileHandle::create(path)?;
    let written = handle.write_all(content.as_bytes())?;
    println!("Zapisano {written} bajtow do {path}");
    Ok(())
}
