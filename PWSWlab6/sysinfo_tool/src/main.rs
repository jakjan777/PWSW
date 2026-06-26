mod file_report;
mod registry;
mod system_info;

use windows::core::Result;

fn main() -> Result<()> {
    println!("sysinfo_tool\n");

    let cpu_count = system_info::get_cpu_count();
    let mem_gb = system_info::get_available_memory_gb();
    let product = registry::read_registry_string(
        "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion",
        "ProductName",
    )?;

    let report = format!(
        "Procesory: {cpu_count}\nPamiec dostepna: {mem_gb} GB\nSystem: {product}\n"
    );

    file_report::write_report_to_file("sysinfo.txt", &report)?;
    println!();
    print!("{report}");

    Ok(())
}
