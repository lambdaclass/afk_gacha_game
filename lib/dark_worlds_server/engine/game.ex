defmodule DarkWorldsServer.Engine.Game do
  alias DarkWorldsServer.Engine.{Board, Player}

  @enforce_keys [:players, :board]
  defstruct [:players, :board]

  def new(number_of_players: number_of_players, board: {width, height}) do
    players =
      Enum.map(1..number_of_players, fn number ->
        Player.new(number, 100)
      end)

    board = Board.new(width, height)

    {board, players} =
      players
      |> Enum.reduce(
        {board, players},
        fn player, {board, players} ->
          {new_board, new_player} = Board.add_player(board, player)
          {new_board, players |> update_player(new_player)}
        end
      )

    new(players, board)
  end

  def new(players, board) do
    %__MODULE__{
      players: players,
      board: board
    }
  end

  use Rustler, otp_app: :dark_worlds_server, crate: "gamestate"

  def new_game(_a, _b, _c), do: :erlang.nif_error(:nif_not_loaded)

  defp update_player(players, %Player{} = player) do
    Enum.map(players, fn p ->
      if p.number == player.number do
        player
      else
        p
      end
    end)
  end
end
