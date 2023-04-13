defmodule DarkWorldsServer.Engine.Runner do
  use GenServer

  alias DarkWorldsServer.Engine.Game
  alias DarkWorldsServer.Engine.{Action}

  @players 2
  @board {5, 5}

  def start_link(args) do
    GenServer.start_link(__MODULE__, args, name: __MODULE__)
  end

  def init(_opts) do
    state = Game.new(number_of_players: @players, board: @board)
    IO.inspect(state)
    {:ok, state}
  end

  def play(%Action{} = action) do
    __MODULE__ |> GenServer.cast({:play, action})
  end

  def handle_cast({:play, %Action{action: "MOVE"} = action}, state) do
    {
      :noreply,
      state
      |> Game.move_player(action.player, action.value)
      |> IO.inspect()
    }
  end
end
