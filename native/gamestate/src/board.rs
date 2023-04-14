use rustler::NifStruct;

#[derive(Debug, Clone, NifStruct)]
#[module = "DarkWorldsServer.Engine.Board"]
pub struct Board {
    pub width: usize,
    pub height: usize,
    pub grid: Vec<Vec<u64>>,
}

impl Board {
    pub fn new(grid: Vec<Vec<u64>>, width: usize, height: usize) -> Self {
        Self {
            grid,
            width,
            height,
        }
    }

    pub fn get_cell(self: &Self, row_idx: usize, col_idx: usize) -> u64 {
        self.grid[row_idx][col_idx]
    }

    pub fn set_cell(self: &mut Self, row_idx: usize, col_idx: usize, value: u64) {
        self.grid[row_idx][col_idx] = value;
    }
}
