#[derive(Debug, Clone)]
pub struct Player {
    pub id: u64,
    pub health: u64,
    pub position: (usize, usize),
}

impl Player {
    pub fn new(id: u64, health: u64, position: (usize, usize)) -> Self {
        Self {
            id,
            health,
            position,
        }
    }
}
