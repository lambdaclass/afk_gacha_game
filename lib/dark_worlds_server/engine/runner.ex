defmodule DarkWorldsServer.Engine.Runner do
  use GenServer, restart: :transient

  alias DarkWorldsServer.Engine.Game
  alias DarkWorldsServer.Engine.{ActionOk}

  @players 2
  @board {10, 10}
  # The game will be closed five minute after it starts
  @game_timeout 5 * 60 * 1000
  # The session will be closed one minute after the game has finished
  @session_timeout 60 * 1000

  def start_link(args) do
    GenServer.start_link(__MODULE__, args)
  end

  def init(_opts) do
    game = Game.new(number_of_players: @players, board: @board)
    IO.inspect("To join: #{encode_pid(self())}")
    Process.send_after(self(), :game_timeout, @game_timeout)
    {:ok, %{game: game, has_finished?: false}}
  end

  def play(runner_pid, %ActionOk{} = action) do
    GenServer.cast(runner_pid, {:play, action})
  end

  def get_board(runner_pid) do
    GenServer.call(runner_pid, :get_board)
  end

  def get_players(runner_pid) do
    GenServer.call(runner_pid, :get_players)
  end

  def handle_cast(
        {:play, %ActionOk{action: :move, player: player, value: value}},
        %{game: game} = state
      ) do
    game =
      game
      |> Game.move_player(player, value)

    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast("game_play_#{encode_pid(self())}", {:move, game.board})

    {:noreply, Map.put(state, :game, game)}
  end

  def handle_cast(
        {:play, %ActionOk{action: :attack, player: player, value: value}},
        %{game: game} = state
      ) do
    game =
      game
      |> Game.attack_player(player, value)

    has_a_player_won? = has_a_player_won?(game.players)
    maybe_broadcast_game_finished_message(has_a_player_won?, game)

    state = state |> Map.put(:game, game) |> Map.put(:has_finished?, has_a_player_won?)

    {:noreply, state}
  end

  def handle_cast(
        {:play, %ActionOk{action: :attack_aoe, player: player, value: value}},
        %{game: game} = state
      ) do
    game =
      game
      |> Game.attack_aoe(player, value)

    has_a_player_won? = has_a_player_won?(game.players)
    maybe_broadcast_game_finished_message(has_a_player_won?, game)

    state = state |> Map.put(:game, game) |> Map.put(:has_finished?, has_a_player_won?)

    {:noreply, state}
  end

  def handle_call(:get_board, _from, %{game: %Game{board: board}} = state) do
    {:reply, board, state}
  end

  def handle_call(:get_players, _from, %{game: %Game{players: players}} = state) do
    {:reply, players, state}
  end

  def handle_info(:game_timeout, state) do
    Process.send_after(self(), :session_timeout, @session_timeout)

    {:noreply, Map.put(state, :has_finished?, true)}
  end

  def handle_info(:session_timeout, state) do
    IO.inspect(self(), label: "session timeout")

    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast("game_play_#{encode_pid(self())}", {:game_finished, state.game})

    {:stop, :normal, state}
  end

  def encode_pid(pid) do
    pid |> :erlang.term_to_binary() |> Base.encode64()
  end

  def game_has_finished?(pid) do
    %{has_finished?: has_finished?} = :sys.get_state(pid)
    has_finished?
  end

  defp has_a_player_won?(players) do
    players_alive = Enum.filter(players, fn player -> player.health != 0 end)
    Enum.count(players_alive) == 1
  end

  defp maybe_broadcast_game_finished_message(true, game) do
    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast("game_play_#{encode_pid(self())}", {:game_finished, game})

    Process.send_after(self(), :session_timeout, @session_timeout)
  end

  defp maybe_broadcast_game_finished_message(_false, game) do
    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast("game_play_#{encode_pid(self())}", {:attack, game.players})
  end
end
