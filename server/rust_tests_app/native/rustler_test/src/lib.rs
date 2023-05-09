#[allow(dead_code, unused)]
pub mod game_state_test;
use gamestate::board::GridResource;
use gamestate::board::Tile;
use gamestate::game::MELEE_ATTACK_COOLDOWN;
use gamestate::game::{GameState, Direction};
use gamestate::player::Player;
use gamestate::player::Position;
use gamestate::time_utils;
use game_state_test::*;

rustler::init!("Elixir.TestNIFs", [
    game_state_test::no_move_if_beyond_boundaries,
    game_state_test::no_move_if_occupied,
    game_state_test::attacking,
    game_state_test::no_move_if_wall,
    game_state_test::movement,
], load = gamestate::load);
