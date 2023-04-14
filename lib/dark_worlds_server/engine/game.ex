defmodule DarkWorldsServer.Engine.Game do
  alias DarkWorldsServer.Engine.{Board, Player}

  @enforce_keys [:players, :board]
  defstruct [:players, :board]

  def new(number_of_players: number_of_players, board: {width, height}) do
    new_game(number_of_players, width, height)
  end

  use Rustler, otp_app: :dark_worlds_server, crate: "gamestate"

  def new_game(_a, _b, _c), do: :erlang.nif_error(:nif_not_loaded)
  def move_player(_a, _b, _c), do: :erlang.nif_error(:nif_not_loaded)

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
