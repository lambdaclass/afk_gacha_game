use rustler::{NifStruct, NifTaggedEnum};

#[derive(Debug, Clone, NifStruct)]
#[module = "DarkWorldsServer.Engine.Board"]
pub struct Board {
    pub width: usize,
    pub height: usize,
    pub grid: Vec<Vec<Tile>>,
}

#[derive(Debug, Clone, NifTaggedEnum, PartialEq)]
pub enum Tile {
    Player(u64),
    Empty,
    Wall,
}

impl Board {
    pub fn new(width: usize, height: usize) -> Self {
        let grid = vec![vec![Tile::Empty; height]; width];

        Self {
            grid,
            width,
            height,
        }
    }

    pub fn get_cell(self: &Self, row_idx: usize, col_idx: usize) -> Option<&Tile> {
        if let Some(row) = self.grid.get(row_idx) {
            row.get(col_idx)
        } else {
            None
        }
    }

    pub fn set_cell(self: &mut Self, row_idx: usize, col_idx: usize, value: Tile) {
        self.grid[row_idx][col_idx] = value;
    }
}
