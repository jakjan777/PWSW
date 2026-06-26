mod logger;
mod service;

use std::ffi::OsString;
use windows_service::{define_windows_service, service_dispatcher};

const SERVICE_NAME: &str = "RustFileMonitor";

define_windows_service!(ffi_service_main, service_main);

fn main() -> Result<(), Box<dyn std::error::Error>> {
    env_logger::init();
    service_dispatcher::start(SERVICE_NAME, ffi_service_main)?;
    Ok(())
}

fn service_main(_arguments: Vec<OsString>) {
    if let Err(err) = service::run_service() {
        logger::log_event(&format!("Blad serwisu: {err}"));
    }
}
