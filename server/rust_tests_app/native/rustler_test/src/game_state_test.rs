use gamestate::board::GridResource;
use gamestate::board::Tile;
use gamestate::game::MELEE_ATTACK_COOLDOWN;
use gamestate::game::{GameState, Direction};
use gamestate::player::Player;
use gamestate::player::Position;
use gamestate::time_utils;

fn get_grid(game: &GameState) -> Vec<Vec<Tile>> {
    let grid = game.board.grid.resource.lock().unwrap();
    grid.clone()
}

#[rustler::nif]
pub fn no_move_if_beyond_boundaries() {
    let mut expected_grid: Vec<Vec<Tile>>;
    let mut state = GameState::new(1, 2, 2, false);
    let player_id = state.players.first().unwrap().id;

    // Check UP boundary
    state.move_player(player_id, Direction::UP);
    assert_eq!(0, state.players.first().unwrap().position.x);
    expected_grid = get_grid(&state);
    state.move_player(player_id, Direction::UP);
    assert_eq!(expected_grid, get_grid(&state));

    // Check DOWN boundary
    state.move_player(player_id, Direction::DOWN);
    assert_eq!(1, state.players.first().unwrap().position.x);

    expected_grid = get_grid(&state);
    state.move_player(player_id, Direction::DOWN);
    assert_eq!(expected_grid, get_grid(&state));

    // Check RIGHT boundary
    state.move_player(player_id, Direction::RIGHT);
    assert_eq!(1, state.players.first().unwrap().position.y);

    expected_grid = get_grid(&state);
    state.move_player(player_id, Direction::RIGHT);
    assert_eq!(expected_grid, get_grid(&state));

    // Check LEFT boundary
    state.move_player(player_id, Direction::LEFT);
    assert_eq!(0, state.players.first().unwrap().position.y);

    expected_grid = get_grid(&state);
    state.move_player(player_id, Direction::LEFT);
    assert_eq!(expected_grid, get_grid(&state));
}

#[rustler::nif]
fn no_move_if_occupied() {
    let mut state = GameState::new(2, 2, 2, false);
    let player1_id = 1;
    let player2_id = 2;
    let player1 = Player::new(player1_id, 100, Position::new(0, 0));
    let player2 = Player::new(player2_id, 100, Position::new(0, 1));
    state.players = vec![player1, player2];
    state.board.set_cell(0, 0, Tile::Player(player1_id));
    state.board.set_cell(0, 1, Tile::Player(player2_id));
    state.board.set_cell(1, 1, Tile::Empty);
    state.board.set_cell(1, 0, Tile::Empty);

    let expected_grid = get_grid(&state);
    state.move_player(player1_id, Direction::RIGHT);
    assert_eq!(expected_grid, get_grid(&state));
}

#[rustler::nif]
fn no_move_if_wall() {
    let mut state = GameState::new(1, 2, 2, false);
    let player1_id = 1;
    let player1 = Player::new(player1_id, 100, Position::new(0, 0));
    state.players = vec![player1];
    state.board.set_cell(0, 0, Tile::Player(player1_id));
    state.board.set_cell(0, 1, Tile::Wall);

    let expected_grid = get_grid(&state);
    state.move_player(player1_id, Direction::RIGHT);
    assert_eq!(expected_grid, get_grid(&state));
}

#[rustler::nif]
fn movement() {
    let mut state = GameState::new(0, 2, 2, false);
    let player_id = 1;
    let player1 = Player::new(player_id, 100, Position::new(0, 0));
    state.players = vec![player1];
    state.board.set_cell(0, 0, Tile::Player(player_id));

    state.move_player(player_id, Direction::RIGHT);
    assert_eq!(
        vec![
            vec![Tile::Empty, Tile::Player(player_id)],
            vec![Tile::Empty, Tile::Empty]
        ],
        get_grid(&state)
    );

    state.move_player(player_id, Direction::DOWN);
    assert_eq!(
        vec![
            vec![Tile::Empty, Tile::Empty],
            vec![Tile::Empty, Tile::Player(player_id)]
        ],
        get_grid(&state)
    );

    state.move_player(player_id, Direction::LEFT);
    assert_eq!(
        vec![
            vec![Tile::Empty, Tile::Empty],
            vec![Tile::Player(player_id), Tile::Empty]
        ],
        get_grid(&state)
    );

    state.move_player(player_id, Direction::UP);
    assert_eq!(
        vec![
            vec![Tile::Player(player_id), Tile::Empty],
            vec![Tile::Empty, Tile::Empty]
        ],
        get_grid(&state)
    );
}

#[rustler::nif]
fn attacking() {
    let mut state = GameState::new(0, 2, 2, false);
    let player_1_id = 1;
    let player_2_id = 2;
    let player1 = Player::new(player_1_id, 100, Position::new(0, 0));
    let player2 = Player::new(player_2_id, 100, Position::new(0, 0));
    state.players = vec![player1, player2];
    state.board.set_cell(0, 0, Tile::Player(player_1_id));
    state.board.set_cell(0, 1, Tile::Player(player_2_id));

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
