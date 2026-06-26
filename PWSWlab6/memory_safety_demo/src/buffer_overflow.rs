pub fn demo_buffer_overflow() {
    println!("--- Buffer overflow ---");

    let mut safe_buf = [0u8; 10];
    match safe_buf.get_mut(15) {
        Some(element) => *element = 42,
        None => println!("  Indeks 15 poza zakresem -- obsluzono bezpiecznie"),
    }

    if let Some(val) = safe_buf.get(5) {
        println!("  Wartosc pod indeksem 5: {val}");
    }
}
