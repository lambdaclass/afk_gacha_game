mod board;
mod game;
mod player;

use game::GameState;

#[rustler::nif(schedule = "DirtyCpu")]
fn new_game(number_of_players: u64, board_width: usize, board_height: usize) -> GameState {
    GameState::new(number_of_players, board_width, board_height)
}

rustler::init!("Elixir.DarkWorldsServer.Engine.Game", [new_game]);
