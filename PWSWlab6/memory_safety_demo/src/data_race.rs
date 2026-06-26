pub fn demo_data_race_blocked() {
    println!("--- Data race ---");

    let counter = 0;

    // let handle = std::thread::spawn(|| { counter += 1; });  // blad kompilacji

    println!("  Data race zablokowany na etapie kompilacji ({counter})");
}
