defmodule DarkWorldsServer.Engine.Runner do
  @enforce_keys [:players, :board]
  defstruct [:players, :board]

  use GenServer

  alias DarkWorldsServer.Engine.{Board, Player}

  def start_link(args) do
    GenServer.start_link(__MODULE__, args, name: __MODULE__)
  end

  def init(_opts) do
    state = %__MODULE__{
      board: Board.new(10, 10),
      players: [
        Player.new(1, 100),
        Player.new(2, 100)
      ]
    }

    {:ok, state}
  end
end
