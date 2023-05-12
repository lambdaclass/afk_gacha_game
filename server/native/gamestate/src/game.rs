use rand::{thread_rng, Rng};
use rustler::{NifStruct, NifUnitEnum};
use std::collections::HashSet;

use crate::board::{Board, Tile};
use crate::player::{Player, Position, Status};
use crate::time_utils::time_now;
use std::cmp::{max, min};

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
    pub fn new(
        number_of_players: u64,
        board_width: usize,
        board_height: usize,
        build_walls: bool,
    ) -> Self {
        let mut positions = HashSet::new();
        let players: Vec<Player> = (1..number_of_players + 1)
            .map(|player_id| {
                let new_position = generate_new_position(&mut positions, board_width, board_height);
                Player::new(player_id, 100, new_position)
            })
            .collect();

        let mut board = Board::new(board_width, board_height);

        for player in players.clone() {
            board.set_cell(
                player.position.x,
                player.position.y,
                Tile::Player(player.id),
            );
        }

        // We generate 10 random walls if walls is true
        if build_walls {
            for _ in 1..=10 {
                let rng = &mut thread_rng();
                let row_idx: usize = rng.gen_range(0..board_width);
                let col_idx: usize = rng.gen_range(0..board_height);
                if let Some(Tile::Empty) = board.get_cell(row_idx, col_idx) {
                    board.set_cell(row_idx, col_idx, Tile::Wall);
                }
            }
        }

        Self { players, board }
    }

    pub fn move_player(self: &mut Self, player_id: u64, direction: Direction) {
        let player = self
            .players
            .iter_mut()
            .find(|player| player.id == player_id)
            .unwrap();

        if matches!(player.status, Status::DEAD) {
            return;
        }

        let mut new_position = compute_adjacent_position(&direction, &player.position);

        // These changes are done so that if the player is moving into one of the map's borders
        // but is not already on the edge, they move to the edge. In simpler terms, if the player is
        // trying to move from (0, 1) to the left, this ensures that new_position is (0, 0) instead of
        // something invalid like (0, -1).
        new_position.x = min(new_position.x, self.board.height - 1);
        new_position.x = max(new_position.x, 0);
        new_position.y = min(new_position.y, self.board.width - 1);
        new_position.y = max(new_position.y, 0);

        if !is_valid_movement(&self.board, &player.position, &new_position) {
            return;
        }

        // Remove the player from their previous position on the board
        self.board
            .set_cell(player.position.x, player.position.y, Tile::Empty);

        player.position = new_position;
        self.board.set_cell(
            player.position.x,
            player.position.y,
            Tile::Player(player.id),
        );
    }

    pub fn attack_player(self: &mut Self, attacking_player_id: u64, attack_direction: Direction) {
        let attacking_player = self
            .players
            .iter_mut()
            .find(|player| player.id == attacking_player_id)
            .unwrap();

        if matches!(attacking_player.status, Status::DEAD) {
            return;
        }

        let now = time_now();

        if (now - attacking_player.last_melee_attack) < MELEE_ATTACK_COOLDOWN {
            return;
        }
        attacking_player.last_melee_attack = now;

        let target_position =
            compute_adjacent_position(&attack_direction, &attacking_player.position);
        let maybe_target_cell = self.board.get_cell(target_position.x, target_position.y);

        if maybe_target_cell.is_none() {
            return;
        }

        if let Some(target_player) = self.players.iter_mut().find(|player| {
            let tile = maybe_target_cell.clone().unwrap();
            match tile {
                Tile::Player(tile_player_id) if tile_player_id == player.id => true,
                _ => false,
            }
        }) {
            modify_health(target_player, -10);
            let player = target_player.clone();
            self.modify_cell_if_player_died(&player);
        }
    }

    // Go over each player, check if they are inside the circle. If they are, damage them according
    // to their distance to the center.
    pub fn attack_aoe(self: &mut Self, attacking_player_id: u64, center_of_attack: &Position) {
        for player in self.players.iter_mut() {
            if player.id == attacking_player_id {
                continue;
            }

            let distance = distance_to_center(player, center_of_attack);
            if distance < 3.0 {
                let damage = (((3.0 - distance) / 3.0) * 10.0) as i64;
                modify_health(player, -damage);
            }
        }
    }

    fn remove_dead_players(self: &mut Self) {
        self.players.iter_mut().for_each(|player| {
            if matches!(player.status, Status::DEAD) {
                self.board
                    .set_cell(player.position.x, player.position.y, Tile::Empty);
            }
        })
    }

    fn modify_cell_if_player_died(self: &mut Self, player: &Player) {
        if matches!(player.status, Status::DEAD) {
            self.board
                .set_cell(player.position.x, player.position.y, Tile::Empty);
        }
    }
}

fn modify_health(player: &mut Player, hp_points: i64) {
    if matches!(player.status, Status::ALIVE) {
        player.health += hp_points;
        if player.health <= 0 {
            player.status = Status::DEAD;
        }
    }
}
/// Given a position and a direction, returns the position adjacent to it in that direction.
/// Example: If the arguments are Direction::RIGHT and (0, 0), returns (0, 1).
fn compute_adjacent_position(direction: &Direction, position: &Position) -> Position {
    let x = position.x;
    let y = position.y;

    match direction {
        Direction::UP => Position::new(x.saturating_sub(2), y),
        Direction::DOWN => Position::new(x + 2, y),
        Direction::LEFT => Position::new(x, y.saturating_sub(2)),
        Direction::RIGHT => Position::new(x, y + 2),
    }
}

/// Checks if the given movement from `old_position` to `new_position` is valid.
/// The way we do it is separated into cases but the idea is always the same:
/// First of all check that we are not trying to move away from the board.
/// Then go through the tiles that are between the new_position and the old_position
/// and ensure that each one of them is empty. If that's not the case, the movement is
/// invalid; otherwise it's valid.
/// The cases that we separate the check into are the following:
/// - Movement is in the Y direction. This is divided into two other cases:
///     - Movement increases the Y coordinate (new_position.y > old_position.y).
///     - Movement decreases the Y coordinate (new_position.y < old_position.y).
/// - Movement is in the X direction. This is also divided into two cases:
///     - Movement increases the X coordinate (new_position.x > old_position.x).
///     - Movement decreases the X coordinate (new_position.x < old_position.x).
fn is_valid_movement(board: &Board, old_position: &Position, new_position: &Position) -> bool {
    if new_position.x > (board.height - 1) || new_position.y > (board.width - 1) {
        return false;
    }

    if new_position.x == old_position.x {
        if new_position.y > old_position.y {
            for i in 1..(new_position.y - old_position.y) + 1 {
                let cell = board.get_cell(old_position.x, old_position.y + i);

                match cell {
                    Some(Tile::Empty) => continue,
                    None => continue,
                    Some(_) => return false,
                }
            }
        } else {
            for i in 1..(old_position.y - new_position.y) + 1 {
                let cell = board.get_cell(old_position.x, old_position.y - i);

                match cell {
                    Some(Tile::Empty) => continue,
                    None => continue,
                    Some(_) => return false,
                }
            }
        }
    } else {
        if new_position.x > old_position.x {
            for i in 1..(new_position.x - old_position.x) + 1 {
                let cell = board.get_cell(old_position.x + i, old_position.y);

                match cell {
                    Some(Tile::Empty) => continue,
                    None => continue,
                    Some(_) => return false,
                }
            }
        } else {
            for i in 1..(old_position.x - new_position.x) + 1 {
                let cell = board.get_cell(old_position.x - i, old_position.y);

                match cell {
                    Some(Tile::Empty) => continue,
                    None => continue,
                    Some(_) => return false,
                }
            }
        }
    }

    true
}

fn distance_to_center(player: &Player, center: &Position) -> f64 {
    let distance_squared =
        (player.position.x - center.x).pow(2) + (player.position.y - center.y).pow(2);
    (distance_squared as f64).sqrt()
}

fn generate_new_position(
    positions: &mut HashSet<(usize, usize)>,
    board_width: usize,
    board_height: usize,
) -> Position {
    let rng = &mut thread_rng();
    let mut x_coordinate: usize = rng.gen_range(0..board_width);
    let mut y_coordinate: usize = rng.gen_range(0..board_height);

    while positions.contains(&(x_coordinate, y_coordinate)) {
        x_coordinate = rng.gen_range(0..board_width);
        y_coordinate = rng.gen_range(0..board_height);
    }

    positions.insert((x_coordinate, y_coordinate));
    Position::new(x_coordinate, y_coordinate)
}

// #[cfg(test)]
// mod tests {
//     use rustler::Env;
//     use rustler::Term;

//     use crate::board::GridResource;
//     use crate::board::Tile;
//     use crate::game::MELEE_ATTACK_COOLDOWN;
//     use crate::player::Player;
//     use crate::player::Position;
//     use crate::time_utils;

//     use super::Direction;
//     use super::GameState;

//     fn get_grid(game: &GameState) -> Vec<Vec<Tile>> {
//         let grid = game.board.grid.resource.lock().unwrap();
//         grid.clone()
//     }

//     #[test]
//     fn no_move_if_beyond_boundaries() {
//         let mut expected_grid: Vec<Vec<Tile>>;
//         let mut state = GameState::new(1, 2, 2, false);
//         let player_id = state.players.first().unwrap().id;

//         // Check UP boundary
//         state.move_player(player_id, Direction::UP);
//         assert_eq!(0, state.players.first().unwrap().position.x);

//         expected_grid = get_grid(&state);
//         state.move_player(player_id, Direction::UP);
//         assert_eq!(expected_grid, get_grid(&state));

//         // Check DOWN boundary
//         state.move_player(player_id, Direction::DOWN);
//         assert_eq!(1, state.players.first().unwrap().position.x);

//         expected_grid = get_grid(&state);
//         state.move_player(player_id, Direction::DOWN);
//         assert_eq!(expected_grid, get_grid(&state));

//         // Check RIGHT boundary
//         state.move_player(player_id, Direction::RIGHT);
//         assert_eq!(1, state.players.first().unwrap().position.y);

//         expected_grid = get_grid(&state);
//         state.move_player(player_id, Direction::RIGHT);
//         assert_eq!(expected_grid, get_grid(&state));

//         // Check LEFT boundary
//         state.move_player(player_id, Direction::LEFT);
//         assert_eq!(0, state.players.first().unwrap().position.y);

//         expected_grid = get_grid(&state);
//         state.move_player(player_id, Direction::LEFT);
//         assert_eq!(expected_grid, get_grid(&state));
//     }

//     #[test]
//     fn no_move_if_occupied() {
//         let mut state = GameState::new(2, 2, 2, false);
//         let player1_id = 1;
//         let player2_id = 2;
//         let player1 = Player::new(player1_id, 100, Position::new(0, 0));
//         let player2 = Player::new(player2_id, 100, Position::new(0, 1));
//         state.players = vec![player1, player2];
//         state.board.set_cell(0, 0, Tile::Player(player1_id));
//         state.board.set_cell(0, 1, Tile::Player(player2_id));
//         state.board.set_cell(1, 1, Tile::Empty);
//         state.board.set_cell(1, 0, Tile::Empty);

//         let expected_grid = get_grid(&state);
//         state.move_player(player1_id, Direction::RIGHT);
//         assert_eq!(expected_grid, get_grid(&state));
//     }

//     #[test]
//     fn no_move_if_wall() {
//         let mut state = GameState::new(1, 2, 2, false);
//         let player1_id = 1;
//         let player1 = Player::new(player1_id, 100, Position::new(0, 0));
//         state.players = vec![player1];
//         state.board.set_cell(0, 0, Tile::Player(player1_id));
//         state.board.set_cell(0, 1, Tile::Wall);

//         let expected_grid = get_grid(&state);
//         state.move_player(player1_id, Direction::RIGHT);
//         assert_eq!(expected_grid, get_grid(&state));
//     }

//     #[test]
//     fn movement() {
//         let mut state = GameState::new(0, 2, 2, false);
//         let player_id = 1;
//         let player1 = Player::new(player_id, 100, Position::new(0, 0));
//         state.players = vec![player1];
//         state.board.set_cell(0, 0, Tile::Player(player_id));

//         state.move_player(player_id, Direction::RIGHT);
//         assert_eq!(
//             vec![
//                 vec![Tile::Empty, Tile::Player(player_id)],
//                 vec![Tile::Empty, Tile::Empty]
//             ],
//             get_grid(&state)
//         );

//         state.move_player(player_id, Direction::DOWN);
//         assert_eq!(
//             vec![
//                 vec![Tile::Empty, Tile::Empty],
//                 vec![Tile::Empty, Tile::Player(player_id)]
//             ],
//             get_grid(&state)
//         );

//         state.move_player(player_id, Direction::LEFT);
//         assert_eq!(
//             vec![
//                 vec![Tile::Empty, Tile::Empty],
//                 vec![Tile::Player(player_id), Tile::Empty]
//             ],
//             get_grid(&state)
//         );

//         state.move_player(player_id, Direction::UP);
//         assert_eq!(
//             vec![
//                 vec![Tile::Player(player_id), Tile::Empty],
//                 vec![Tile::Empty, Tile::Empty]
//             ],
//             get_grid(&state)
//         );
//     }

//     #[test]
//     fn attacking() {
//         let mut state = GameState::new(0, 2, 2, false);
//         let player_1_id = 1;
//         let player_2_id = 2;
//         let player1 = Player::new(player_1_id, 100, Position::new(0, 0));
//         let player2 = Player::new(player_2_id, 100, Position::new(0, 0));
//         state.players = vec![player1, player2];
//         state.board.set_cell(0, 0, Tile::Player(player_1_id));
//         state.board.set_cell(0, 1, Tile::Player(player_2_id));

//         time_utils::sleep(MELEE_ATTACK_COOLDOWN);

//         // Attack lands and damages player
//         state.attack_player(player_1_id, Direction::RIGHT);
//         assert_eq!(100, state.players[0].health);
//         assert_eq!(90, state.players[1].health);

//         // Attack does nothing because of cooldown
//         state.attack_player(player_1_id, Direction::RIGHT);
//         assert_eq!(100, state.players[0].health);
//         assert_eq!(90, state.players[1].health);

//         time_utils::sleep(MELEE_ATTACK_COOLDOWN);

//         // Attack misses and does nothing
//         state.attack_player(player_1_id, Direction::DOWN);
//         assert_eq!(100, state.players[0].health);
//         assert_eq!(90, state.players[1].health);

//         time_utils::sleep(MELEE_ATTACK_COOLDOWN);

//         state.move_player(player_1_id, Direction::DOWN);

//         // Attacking to the right now does nothing since the player moved down.
//         state.attack_player(player_1_id, Direction::RIGHT);
//         assert_eq!(100, state.players[0].health);
//         assert_eq!(90, state.players[1].health);

//         time_utils::sleep(MELEE_ATTACK_COOLDOWN);

//         // Attacking to a non-existent position on the board does nothing.
//         state.attack_player(player_1_id, Direction::LEFT);
//         assert_eq!(100, state.players[0].health);
//         assert_eq!(90, state.players[1].health);
//     }
// }
