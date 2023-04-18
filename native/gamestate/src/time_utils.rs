use std::time::{SystemTime, UNIX_EPOCH};
use std::{thread, time};

/// Returns the current system time in seconds. Note that system time is
/// unreliable as it's not guaranteed to be monotonic.
pub fn time_now() -> u64 {
    let start = SystemTime::now();
    let since_the_epoch = start
        .duration_since(UNIX_EPOCH)
        .expect("Time went backwards");

    since_the_epoch.as_secs()
}

pub fn sleep(seconds: u64) {
    let duration = time::Duration::from_secs(seconds);
    thread::sleep(duration);
}
