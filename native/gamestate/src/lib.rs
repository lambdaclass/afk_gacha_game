mod board;
mod game;
mod player;
mod time_utils;

use game::GameState;

use crate::{game::Direction, player::Position};

#[rustler::nif(schedule = "DirtyCpu")]
fn new_game(
    number_of_players: u64,
    board_width: usize,
    board_height: usize,
    build_walls: bool,
) -> GameState {
    GameState::new(number_of_players, board_width, board_height, build_walls)
}

#[rustler::nif(schedule = "DirtyCpu")]
fn move_player(game: GameState, player_id: u64, direction: Direction) -> GameState {
    let mut game_2 = game;
    game_2.move_player(player_id, direction);
    game_2
}

#[rustler::nif(schedule = "DirtyCpu")]
fn attack_player(
    game: GameState,
    attacking_player_id: u64,
    attack_direction: Direction,
) -> GameState {
    let mut game_2 = game;
    game_2.attack_player(attacking_player_id, attack_direction);
    game_2
}

#[rustler::nif(schedule = "DirtyCpu")]
fn attack_aoe(game: GameState, attacking_player_id: u64, center_of_attack: Position) -> GameState {
    let mut game_2 = game;
    game_2.attack_aoe(attacking_player_id, &center_of_attack);
    game_2
}

rustler::init!(
    "Elixir.DarkWorldsServer.Engine.Game",
    [new_game, move_player, attack_player, attack_aoe]
);
