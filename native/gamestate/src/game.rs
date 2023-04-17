use rand::{thread_rng, Rng};
use rustler::{NifStruct, NifUnitEnum};

use crate::board::Board;
use crate::player::Player;
use crate::time_utils::time_now;

const MELEE_ATTACK_COOLDOWN: u64 = 1;

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

        let new_position = compute_adjacent_position(direction, player.position);
        if !is_valid_movement(&self.board, new_position) {
            return;
        }

        // Remove the player from their previous position on the board
        self.board.set_cell(player.position.0, player.position.1, 0);

        player.position = new_position;
        self.board
            .set_cell(player.position.0, player.position.1, player.id);
    }

    pub fn attack_player(self: &mut Self, attacking_player_id: u64, attack_direction: Direction) {
        let attacking_player = self
            .players
            .iter_mut()
            .find(|player| player.id == attacking_player_id)
            .unwrap();

        let now = time_now();

        if (now - attacking_player.last_melee_attack) < MELEE_ATTACK_COOLDOWN {
            return;
        }
        attacking_player.last_melee_attack = now;

        let (target_position_x, target_position_y) =
            compute_adjacent_position(attack_direction, attacking_player.position);
        let maybe_target_cell = self.board.get_cell(target_position_x, target_position_y);

        if maybe_target_cell.is_none() {
            return;
        }

        if let Some(target_player) = self
            .players
            .iter_mut()
            .find(|player| player.id == *maybe_target_cell.unwrap())
        {
            target_player.health -= 10;
        }
    }
}

/// Given a position and a direction, returns the position adjacent to it in that direction.
/// Example: If the arguments are Direction::RIGHT and (0, 0), returns (0, 1).
fn compute_adjacent_position(direction: Direction, position: (usize, usize)) -> (usize, usize) {
    let (x, y) = position;

    match direction {
        Direction::UP => (x.wrapping_sub(1), y),
        Direction::DOWN => (x + 1, y),
        Direction::LEFT => (x, y.wrapping_sub(1)),
        Direction::RIGHT => (x, y + 1),
    }
}

fn is_valid_movement(board: &Board, new_position: (usize, usize)) -> bool {
    let (row_idx, col_idx) = new_position;

    let cell = board.get_cell(row_idx, col_idx);
    if cell.is_none() {
        return false;
    }

    // Check if cell is not-occupied
    // This unwrap is safe since we checked for None in the line above.
    if *cell.unwrap() != 0 {
        return false;
    }

    true
}

#[cfg(test)]
mod tests {
    use crate::game::MELEE_ATTACK_COOLDOWN;
    use crate::player::Player;
    use crate::time_utils;

    use super::Direction;
    use super::GameState;

    #[test]
    fn no_move_if_beyond_boundaries() {
        let mut expected_grid: Vec<Vec<u64>>;
        let mut state = GameState::new(1, 2, 2);
        let player_id = state.players.first().unwrap().id;

        // Check UP boundary
        state.move_player(player_id, Direction::UP);
        assert_eq!(0, state.players.first().unwrap().position.0);

        expected_grid = state.board.grid.clone();
        state.move_player(player_id, Direction::UP);
        assert_eq!(expected_grid, state.board.grid);

        // Check DOWN boundary
        state.move_player(player_id, Direction::DOWN);
        assert_eq!(1, state.players.first().unwrap().position.0);

        expected_grid = state.board.grid.clone();
        state.move_player(player_id, Direction::DOWN);
        assert_eq!(expected_grid, state.board.grid);

        // Check RIGHT boundary
        state.move_player(player_id, Direction::RIGHT);
        assert_eq!(1, state.players.first().unwrap().position.1);

        expected_grid = state.board.grid.clone();
        state.move_player(player_id, Direction::RIGHT);
        assert_eq!(expected_grid, state.board.grid);

        // Check LEFT boundary
        state.move_player(player_id, Direction::LEFT);
        assert_eq!(0, state.players.first().unwrap().position.1);

        expected_grid = state.board.grid.clone();
        state.move_player(player_id, Direction::LEFT);
        assert_eq!(expected_grid, state.board.grid);
    }

    #[test]
    fn no_move_if_occupied() {
        let mut state = GameState::new(2, 2, 2);
        let player1_id = 1;
        let player2_id = 2;
        let player1 = Player::new(player1_id, 100, (0, 0));
        let player2 = Player::new(player2_id, 100, (0, 1));
        state.players = vec![player1, player2];
        state.board.set_cell(0, 0, player1_id);
        state.board.set_cell(0, 1, player2_id);
        state.board.set_cell(1, 1, 0);
        state.board.set_cell(1, 0, 0);

        let expected_grid = state.board.grid.clone();
        state.move_player(player1_id, Direction::RIGHT);
        assert_eq!(expected_grid, state.board.grid);
    }

    #[test]
    fn movement() {
        let mut state = GameState::new(0, 2, 2);
        let player_id = 1;
        let player1 = Player::new(player_id, 100, (0, 0));
        state.players = vec![player1];
        state.board.set_cell(0, 0, player_id);

        state.move_player(player_id, Direction::RIGHT);
        assert_eq!(vec![vec![0, player_id], vec![0, 0]], state.board.grid);

        state.move_player(player_id, Direction::DOWN);
        assert_eq!(vec![vec![0, 0], vec![0, player_id]], state.board.grid);

        state.move_player(player_id, Direction::LEFT);
        assert_eq!(vec![vec![0, 0], vec![player_id, 0]], state.board.grid);

        state.move_player(player_id, Direction::UP);
        assert_eq!(vec![vec![player_id, 0], vec![0, 0]], state.board.grid);
    }

    #[test]
    fn attacking() {
        let mut state = GameState::new(0, 2, 2);
        let player_1_id = 1;
        let player_2_id = 2;
        let player1 = Player::new(player_1_id, 100, (0, 0));
        let player2 = Player::new(player_2_id, 100, (0, 0));
        state.players = vec![player1, player2];
        state.board.set_cell(0, 0, player_1_id);
        state.board.set_cell(0, 1, player_2_id);

        time_utils::sleep(MELEE_ATTACK_COOLDOWN);

        // Attack lands and damages player
        state.attack_player(player_1_id, Direction::RIGHT);
        assert_eq!(100, state.players[0].health);
        assert_eq!(90, state.players[1].health);

        // Attack does nothing because of cooldown
        state.attack_player(player_1_id, Direction::RIGHT);
        assert_eq!(100, state.players[0].health);
        assert_eq!(90, state.players[1].health);

        time_utils::sleep(MELEE_ATTACK_COOLDOWN);

        // Attack misses and does nothing
        state.attack_player(player_1_id, Direction::DOWN);
        assert_eq!(100, state.players[0].health);
        assert_eq!(90, state.players[1].health);

        time_utils::sleep(MELEE_ATTACK_COOLDOWN);

        state.move_player(player_1_id, Direction::DOWN);

        // Attacking to the right now does nothing since the player moved down.
        state.attack_player(player_1_id, Direction::RIGHT);
        assert_eq!(100, state.players[0].health);
        assert_eq!(90, state.players[1].health);

        time_utils::sleep(MELEE_ATTACK_COOLDOWN);

        // Attacking to a non-existent position on the board does nothing.
        state.attack_player(player_1_id, Direction::LEFT);
        assert_eq!(100, state.players[0].health);
        assert_eq!(90, state.players[1].health);
    }
}
