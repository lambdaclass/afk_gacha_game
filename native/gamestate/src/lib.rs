mod board;
mod game;
mod player;

use game::GameState;

use crate::game::Direction;

#[rustler::nif(schedule = "DirtyCpu")]
fn new_game(number_of_players: u64, board_width: usize, board_height: usize) -> GameState {
    GameState::new(number_of_players, board_width, board_height)
}

#[rustler::nif(schedule = "DirtyCpu")]
fn move_player(game: GameState, player_id: u64, direction: Direction) -> GameState {
    let mut game_2 = game;
    game_2.move_player(player_id, direction);
    game_2
}

rustler::init!(
    "Elixir.DarkWorldsServer.Engine.Game",
    [new_game, move_player]
);
