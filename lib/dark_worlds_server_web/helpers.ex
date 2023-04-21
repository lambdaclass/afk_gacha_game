defmodule DarkWorldsServerWeb.Helpers do

  def order_players_by_health(players) do
    players
    |> Enum.sort_by(fn player -> player.health end, :desc)
    |> Enum.with_index()
  end

  def alive_players(players) do
    players
    |> Enum.filter(fn player -> is_alive?(player) end)
  end

  def is_alive?(%{state: :alive}), do: true
  def is_alive(_), do: false
end
