pub fn demo_use_after_free() {
    println!("--- Use-after-free ---");

    let data = String::from("poufne dane");
    let reference = &data;

    // drop(data);  // blad kompilacji: nie mozna usunac podczas wypozyczenia
    // println!("{reference}");  // byloby use-after-free bez systemu ownership

    println!("  Referencja jest bezpieczna: {reference}");
}

pub fn demo_ownership_transfer() {
    println!("--- Ownership transfer ---");

    let data = String::from("hello");
    let moved_data = data;

    // println!("{data}");  // blad kompilacji: data zostala przeniesiona
    println!("  Nowy wlasciciel: {moved_data}");
}
