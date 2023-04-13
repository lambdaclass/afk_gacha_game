defmodule DarkWorldsServer.Engine.Board do
  import Nx, only: [is_tensor: 1]
  alias DarkWorldsServer.Engine.Player

  @enforce_keys [:grid]
  defstruct [:grid]

  def new(width, height) when is_integer(width) and is_integer(height) do
    grid =
      List.duplicate(0, width * height)
      |> Enum.chunk_every(width)
      |> Nx.tensor(names: [:y, :x], type: {:u, 8})

    %__MODULE__{grid: grid}
  end

  def new(grid) when is_tensor(grid) do
    %__MODULE__{grid: grid}
  end

  def add_player(%__MODULE__{grid: grid} = board, %Player{number: number} = player) do
    rand_y = Enum.random(0..(get_height(board) - 1))
    rand_x = Enum.random(0..(get_width(board) - 1))

    case Nx.to_flat_list(grid[rand_y][rand_x]) do
      [0] ->
        {
          grid
          |> Nx.put_slice([rand_y, rand_x], Nx.tensor([[number]]))
          |> new(),
          %Player{player | position: {rand_y, rand_x}}
        }

      _ ->
        add_player(board, player)
    end
  end

  def move_player(
        %__MODULE__{grid: grid} = board,
        %Player{number: number, position: {y, x}} = player,
        direction
      ) do
    case direction do
      "UP" ->
        {
          grid
          |> Nx.put_slice([y - 1, x], Nx.tensor([[number]]))
          |> Nx.put_slice([y, x], Nx.tensor([[0]]))
          |> new(),
          %Player{player | position: {y - 1, x}}
        }

      "DOWN" ->
        {
          grid
          |> Nx.put_slice([y + 1, x], Nx.tensor([[number]]))
          |> Nx.put_slice([y, x], Nx.tensor([[0]]))
          |> new(),
          %Player{player | position: {y + 1, x}}
        }

      "LEFT" ->
        {
          grid
          |> Nx.put_slice([y, x - 1], Nx.tensor([[number]]))
          |> Nx.put_slice([y, x], Nx.tensor([[0]]))
          |> new(),
          %Player{player | position: {y, x - 1}}
        }

      "RIGHT" ->
        {
          grid
          |> Nx.put_slice([y, x + 1], Nx.tensor([[number]]))
          |> Nx.put_slice([y, x], Nx.tensor([[0]]))
          |> new(),
          %Player{player | position: {y, x + 1}}
        }

      _ ->
        {board, player}
    end
  end

  defp get_height(%__MODULE__{} = board) do
    {height, _} = Nx.shape(board.grid)
    height
  end

  defp get_width(%__MODULE__{} = board) do
    {_, width} = Nx.shape(board.grid)
    width
  end
end
