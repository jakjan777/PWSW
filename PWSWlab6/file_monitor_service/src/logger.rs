use std::fs::OpenOptions;
use std::io::Write;

const LOG_PATH: &str = r"C:\MonitoredFolder\service.log";

pub fn log_event(message: &str) {
    log::info!("{message}");

    if let Ok(mut file) = OpenOptions::new()
        .create(true)
        .append(true)
        .open(LOG_PATH)
    {
        let _ = writeln!(file, "{message}");
    }
}
