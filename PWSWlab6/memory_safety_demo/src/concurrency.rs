use std::sync::{Arc, Mutex};
use std::thread;

pub fn demo_safe_concurrency() {
    println!("--- Bezpieczna wspolbieznosc ---");

    let counter = Arc::new(Mutex::new(0));
    let mut handles = vec![];

    for i in 0..10 {
        let counter = Arc::clone(&counter);
        let handle = thread::spawn(move || {
            let mut num = counter.lock().unwrap();
            *num += 1;
            println!("  Watek {i} -- licznik: {num}");
        });
        handles.push(handle);
    }

    for handle in handles {
        handle.join().unwrap();
    }

    println!(
        "  Koncowa wartosc: {} (oczekiwana: 10)",
        *counter.lock().unwrap()
    );
}
