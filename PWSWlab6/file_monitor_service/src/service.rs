use std::collections::HashSet;
use std::sync::mpsc;
use std::time::Duration;

use crate::logger;
use windows_service::service::{
    ServiceControl, ServiceControlAccept, ServiceExitCode, ServiceState, ServiceStatus,
    ServiceType,
};
use windows_service::service_control_handler::{self, ServiceControlHandlerResult};

const SERVICE_NAME: &str = "RustFileMonitor";
const WATCH_PATH: &str = r"C:\MonitoredFolder";

pub fn run_service() -> windows_service::Result<()> {
    let (shutdown_tx, shutdown_rx) = mpsc::channel();

    let event_handler = move |control_event| match control_event {
        ServiceControl::Stop => {
            logger::log_event("Otrzymano sygnal STOP");
            shutdown_tx.send(()).ok();
            ServiceControlHandlerResult::NoError
        }
        ServiceControl::Interrogate => ServiceControlHandlerResult::NoError,
        _ => ServiceControlHandlerResult::NotImplemented,
    };

    let status_handle = service_control_handler::register(SERVICE_NAME, event_handler)?;

    status_handle.set_service_status(ServiceStatus {
        service_type: ServiceType::OWN_PROCESS,
        current_state: ServiceState::Running,
        controls_accepted: ServiceControlAccept::STOP,
        exit_code: ServiceExitCode::Win32(0),
        checkpoint: 0,
        wait_hint: Duration::default(),
        process_id: None,
    })?;

    std::fs::create_dir_all(WATCH_PATH).ok();
    logger::log_event("Serwis uruchomiony");
    logger::log_event(&format!("Monitoruje katalog: {WATCH_PATH}"));

    let mut known_files: HashSet<String> = std::fs::read_dir(WATCH_PATH)
        .into_iter()
        .flatten()
        .filter_map(|entry| entry.ok())
        .map(|entry| entry.file_name().to_string_lossy().to_string())
        .collect();

    loop {
        match shutdown_rx.recv_timeout(Duration::from_secs(5)) {
            Ok(()) | Err(mpsc::RecvTimeoutError::Disconnected) => {
                logger::log_event("Zatrzymywanie serwisu...");
                break;
            }
            Err(mpsc::RecvTimeoutError::Timeout) => {
                if let Ok(entries) = std::fs::read_dir(WATCH_PATH) {
                    for entry in entries.flatten() {
                        let name = entry.file_name().to_string_lossy().to_string();
                        if known_files.insert(name.clone()) {
                            logger::log_event(&format!("Nowy plik wykryty: {name}"));
                        }
                    }
                }
            }
        }
    }

    status_handle.set_service_status(ServiceStatus {
        service_type: ServiceType::OWN_PROCESS,
        current_state: ServiceState::Stopped,
        controls_accepted: ServiceControlAccept::empty(),
        exit_code: ServiceExitCode::Win32(0),
        checkpoint: 0,
        wait_hint: Duration::default(),
        process_id: None,
    })?;

    logger::log_event("Serwis zatrzymany");
    Ok(())
}
