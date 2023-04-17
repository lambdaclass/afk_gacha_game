defmodule DarkWorldsServer.Engine.Runner do
  use GenServer

  alias DarkWorldsServer.Engine.Game
  alias DarkWorldsServer.Engine.{ActionOk}

  @players 2
  @board {10, 10}

  def start_link(args) do
    GenServer.start_link(__MODULE__, args, name: __MODULE__)
  end

  def init(_opts) do
    state = Game.new(number_of_players: @players, board: @board)
    IO.inspect(state)
    {:ok, state}
  end

  def play(%ActionOk{} = action) do
    __MODULE__
    |> GenServer.cast({:play, action})
  end

  def get_board do
    __MODULE__
    |> GenServer.call(:get_board)
  end

  def get_players do
    __MODULE__
    |> GenServer.call(:get_players)
  end

  def handle_cast({:play, %ActionOk{action: :move, player: player, value: value}}, state) do
    state =
      state
      |> Game.move_player(player, value)

    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast("game_play", {:move, state.board})

    {:noreply, state}
  end

  def handle_cast({:play, %ActionOk{action: :attack, player: player, value: value}}, state) do
    state =
      state
      |> Game.attack_player(player, value)

    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast("game_play", {:attack, state.players})

    {:noreply, state}
  end

  def handle_call(:get_board, _from, %Game{board: board} = state) do
    {:reply, board, state}
  end

  def handle_call(:get_players, _from, %Game{players: players} = state) do
    {:reply, players, state}
  end
end
