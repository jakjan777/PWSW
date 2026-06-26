mod file_ops;
mod message;
mod registry;
mod system_info;

fn main() -> windows::core::Result<()> {
    println!("=== Audyt systemu Windows ===\n");
    system_info::print_system_info();

    println!();
    registry::read_windows_version()?;

    println!();
    file_ops::safe_file_operations()?;

    message::show_message(
        "Audit Tool",
        "Audyt zakonczony. Raport zapisano w audit_report.txt",
    )?;
    Ok(())
}
