use windows::core::{HSTRING, Result};
use windows::Win32::UI::WindowsAndMessaging::{
    MessageBoxW, MB_ICONINFORMATION, MB_OK,
};

pub fn show_message(title: &str, text: &str) -> Result<()> {
    let title: HSTRING = title.into();
    let text: HSTRING = text.into();
    unsafe {
        MessageBoxW(None, &text, &title, MB_OK | MB_ICONINFORMATION);
    }
    Ok(())
}
