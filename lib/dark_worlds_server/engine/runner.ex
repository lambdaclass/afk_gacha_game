defmodule DarkWorldsServer.Engine.Runner do
  use GenServer

  alias DarkWorldsServer.Engine.Game
  alias DarkWorldsServer.Engine.{ActionOk}

  @players 2
  @board {10, 10}

  def start_link(args) do
    GenServer.start_link(__MODULE__, args)
  end

  def init(_opts) do
    state = Game.new(number_of_players: @players, board: @board)
    IO.inspect(state)
    IO.inspect("To join: #{self() |> :erlang.term_to_binary |> Base.encode64()}")
    {:ok, state}
  end

  def play(runner_pid, %ActionOk{} = action) do
    GenServer.cast(runner_pid, {:play, action})
  end

  def get_board(runner_pid) do
    GenServer.call(runner_pid, :get_board)
  end

  def handle_cast({:play, %ActionOk{action: :move, player: player, value: value}}, state) do
    state =
      state
      |> Game.move_player(player, value)

    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.broadcast("game_play", {:move, state.board})

    {:noreply, state}
  end

  def handle_call(:get_board, _from, %Game{board: board} = state) do
    {:reply, board, state}
  end
end
