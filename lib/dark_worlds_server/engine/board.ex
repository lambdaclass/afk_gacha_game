defmodule DarkWorldsServer.Engine.Board do

  @enforce_keys [:grid]
  defstruct [:grid]

  alias DarkWorldsServer.Engine.Player

  def new(width, height) when is_integer(width) and is_integer(height) do
    grid = List.duplicate(0,width*height)
    |> Enum.chunk_every(width)
    |> Nx.tensor(names: [:y, :x], type: {:u, 8})

    %__MODULE__{grid: grid}
  end

  def add_player(%__MODULE__{} = board, %Player{} = player) do
    rand_y = Enum.random(1,get_height(board))
    rand_x = Enum.random(1,get_width(board))


  end

  def get_height(%__MODULE__{} = board) do
    {height, _ } = Nx.shape(board.grid)
    height
  end

  def get_width(%__MODULE__{} = board) do
    {_, width} = Nx.shape(board.grid)
    width
  end

end
