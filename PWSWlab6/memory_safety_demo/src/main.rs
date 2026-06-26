mod buffer_overflow;
mod concurrency;
mod data_race;
mod ownership;

fn main() {
    println!("=== Bezpieczenstwo pamieci w Rust ===\n");

    buffer_overflow::demo_buffer_overflow();
    println!();
    ownership::demo_use_after_free();
    println!();
    ownership::demo_ownership_transfer();
    println!();
    data_race::demo_data_race_blocked();
    println!();
    concurrency::demo_safe_concurrency();
}
