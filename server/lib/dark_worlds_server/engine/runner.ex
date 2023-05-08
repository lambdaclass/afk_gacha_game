defmodule DarkWorldsServer.Engine.Runner do
  @moduledoc """
  Receives client state updates and updates
  the overall gamestate accordingly with
  PubSub broadcast.
  """
  use GenServer, restart: :transient

  alias DarkWorldsServer.Communication
  alias DarkWorldsServer.Engine.Game
  alias DarkWorldsServer.Engine.{ActionOk}

  @players 3
  @board {1000, 1000}
  # The game will be closed five minute after it starts
  @game_timeout 20 * 60 * 1000
  # The session will be closed one minute after the game has finished
  @session_timeout 60 * 1000
  # This is the amount of time between updates (30ms)
  @update_time 30

  def start_link(args) do
    GenServer.start_link(__MODULE__, args)
  end

  @doc """
  Starts a new game state, triggers the first
  update and the final game timeout.
  """
  def init(_opts) do
    state = Game.new(number_of_players: @players, board: @board, build_walls: false)
    # Finish game after @game_timeout seconds
    Process.send_after(self(), :game_timeout, @game_timeout)

    initial_state = %{
      game: state,
      has_finished?: false
    }

    Process.send_after(self(), :update_state, @update_time)

    {:ok,
     %{
       current_state: initial_state,
       next_state: initial_state,
       max_players: @players,
       current_players: 0
     }}
  end

  def join(runner_pid) do
    GenServer.call(runner_pid, :join)
  end

  def play(runner_pid, player_id, %ActionOk{} = action) do
    GenServer.cast(runner_pid, {:play, player_id, action})
  end

  def get_board(runner_pid) do
    GenServer.call(runner_pid, :get_board)
  end

  def get_players(runner_pid) do
    GenServer.call(runner_pid, :get_players)
  end

  def handle_cast(_actions, %{current_state: %{has_finished?: true}} = state) do
    {:noreply, state}
  end

  @doc """
  Update game state based on the received action.
  """
  def handle_cast(
        {:play, player, %ActionOk{action: :move, value: value}},
        %{next_state: %{game: game} = next_state} = state
      ) do
    game =
      game
      |> Game.move_player(player, value)

    next_state = Map.put(next_state, :game, game)

    state = Map.put(state, :next_state, next_state)

    {:noreply, state}
  end

  @doc """
  Update game state based on the received action.
  """
  def handle_cast(
        {:play, player, %ActionOk{action: :attack, value: value}},
        %{next_state: %{game: game} = next_state} = state
      ) do
    game =
      game
      |> Game.attack_player(player, value)

    has_a_player_won? = has_a_player_won?(game.players)

    next_state = next_state |> Map.put(:game, game) |> Map.put(:has_finished?, has_a_player_won?)
    state = Map.put(state, :next_state, next_state)

    {:noreply, state}
  end

  @doc """
  Update game state based on the received action.
  """
  def handle_cast(
        {:play, player, %ActionOk{action: :attack_aoe, value: value}},
        %{next_state: %{game: game} = next_state} = state
      ) do
    game =
      game
      |> Game.attack_aoe(player, value)

    has_a_player_won? = has_a_player_won?(game.players)

    next_state = next_state |> Map.put(:game, game) |> Map.put(:has_finished?, has_a_player_won?)
    state = Map.put(state, :next_state, next_state)

    {:noreply, state}
  end

  @doc """
  Update game state based on the received action.
  """
  def handle_cast(
        {:play, player, %ActionOk{action: :update_ping, value: value}},
        state
      ) do
    broadcast_players_ping(player, value)

    {:noreply, state}
  end

  def handle_call(:join, _, %{max_players: max, current_players: current} = state)
      when current < max do
    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast(
      Communication.pubsub_game_topic(self()),
      {:player_joined, state}
    )

    {:reply, {:ok, current + 1}, %{state | current_players: current + 1}}
  end

  def handle_call(:join, _, %{max_players: max, current_players: max} = state) do
    {:reply, {:error, :game_full}, state}
  end

  def handle_call(:get_board, _from, %{current_state: %{game: %Game{board: board}}} = state) do
    {:reply, board, state}
  end

  def handle_call(:get_players, _from, %{current_state: %{game: %Game{players: players}}} = state) do
    {:reply, players, state}
  end

  def handle_info(:game_timeout, state) do
    Process.send_after(self(), :session_timeout, @session_timeout)

    {:noreply, Map.put(state, :has_finished?, true)}
  end

  def handle_info(:session_timeout, state) do
    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast(
      Communication.pubsub_game_topic(self()),
      {:game_finished, state}
    )

    {:stop, :normal, state}
  end

  def handle_info(:update_state, %{current_state: %{has_finished?: true}} = state) do
    {:noreply, state}
  end

  @doc """
  Trigger a state broadcast update.
  If some player has won, start the countdown
  to finish the session.
  Else, trigger another update.
  """
  def handle_info(:update_state, %{next_state: next_state} = state) do
    state = Map.put(state, :current_state, next_state)

    has_a_player_won? = has_a_player_won?(next_state.game.players)

    cond do
      has_a_player_won? ->
        Process.send_after(self(), :session_timeout, @session_timeout)

      not has_a_player_won? ->
        Process.send_after(self(), :update_state, @update_time)
    end

    broadcast_game_update(has_a_player_won?, state)

    {:noreply, state}
  end

  def game_has_finished?(pid) do
    %{current_state: %{has_finished?: has_finished?}} = :sys.get_state(pid)
    has_finished?
  end

  defp has_a_player_won?(players) do
    players_alive = Enum.filter(players, fn player -> player.health != 0 end)
    Enum.count(players_alive) == 1
  end

  @doc """
  Broadcast the current game state to
  each connected player.
  """
  defp broadcast_game_update(_has_finished? = true, state) do
    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast(Communication.pubsub_game_topic(self()), {:game_finished, state})
  end

  defp broadcast_game_update(_has_finished? = false, state) do
    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast(Communication.pubsub_game_topic(self()), {:game_update, state})
  end

  defp broadcast_players_ping(player, ping) do
    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast(
      Communication.pubsub_game_topic(self()),
      {:update_ping, player, ping}
    )
  end
end
