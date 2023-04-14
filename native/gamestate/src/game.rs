use crate::board::Board;
use crate::player::Player;

use rand::prelude::*;

pub struct GameState {
    pub players: Vec<Player>,
    pub board: Board,
}

pub enum Direction {
    UP,
    DOWN,
    LEFT,
    RIGHT,
}

impl GameState {
    pub fn new(number_of_players: u64, board_width: usize, board_height: usize) -> Self {
        let players: Vec<Player> = (1..number_of_players + 1)
            .map(|player_id| {
                let x_coordinate: usize = rand::random();
                let y_coordinate: usize = rand::random();
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

        // Remove the player from their previous position on the board
        self.board.set_cell(player.position.0, player.position.1, 0);

        player.position = new_position(direction, player.position);
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
