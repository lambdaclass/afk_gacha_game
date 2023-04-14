use rand::{thread_rng, Rng};
use rustler::{NifStruct, NifUnitEnum};

use crate::board::Board;
use crate::player::Player;

#[derive(NifStruct)]
#[module = "DarkWorldsServer.Engine.Game"]
pub struct GameState {
    pub players: Vec<Player>,
    pub board: Board,
}

#[derive(Debug, NifUnitEnum)]
pub enum Direction {
    UP,
    DOWN,
    LEFT,
    RIGHT,
}

impl GameState {
    pub fn new(number_of_players: u64, board_width: usize, board_height: usize) -> Self {
        let rng = &mut thread_rng();
        let players: Vec<Player> = (1..number_of_players + 1)
            .map(|player_id| {
                let x_coordinate: usize = rng.gen_range(0..board_width);
                let y_coordinate: usize = rng.gen_range(0..board_height);
                Player::new(player_id, 100, (x_coordinate, y_coordinate))
            })
            .collect();

        let grid = vec![vec![0; board_height]; board_width];
        let mut board = Board::new(grid, board_width, board_height);

        for player in players.clone() {
            let (player_x, player_y) = player.position;
            board.set_cell(player_x, player_y, player.id);
        }

        Self { players, board }
    }

    pub fn move_player(self: &mut Self, player_id: u64, direction: Direction) {
        let player = self
            .players
            .iter_mut()
            .find(|player| player.id == player_id)
            .unwrap();

        let new_position = new_position(direction, player.position);
        if !is_valid_movement(&self.board, new_position) {
            return;
        }

        // Remove the player from their previous position on the board
        self.board.set_cell(player.position.0, player.position.1, 0);

        player.position = new_position;
        self.board
            .set_cell(player.position.0, player.position.1, player.id);
    }
}

fn new_position(direction: Direction, position: (usize, usize)) -> (usize, usize) {
    let (x, y) = position;

    match direction {
        Direction::UP => (x - 1, y),
        Direction::DOWN => (x + 1, y),
        Direction::LEFT => (x, y - 1),
        Direction::RIGHT => (x, y + 1),
    }
}

fn is_valid_movement(board: &Board, new_position: (usize, usize)) -> bool {
    let (row_idx, col_idx) = new_position;

    // Check board boundaries
    // Since we have usizes as types when `new_position` is calculated and 0 - 1 happens
    // it will wrap around to usize::MAX which will alway be greater than `board.height`
    // so we just need to check that it idx are equal or greather than boundaries
    if row_idx >= board.height || col_idx >= board.width {
        return false;
    }

    // Check if cell is not-occupied
    let cell = board.get_cell(row_idx, col_idx);
    if cell != 0 {
        return false;
    }

    true
}
