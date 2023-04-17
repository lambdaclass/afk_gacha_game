defmodule DarkWorldsServerWeb.BoardLive.Index do
  use DarkWorldsServerWeb, :live_view

  alias DarkWorldsServer.Engine.{Runner, Board}

  def mount(_params, _session, socket) do
    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.subscribe("game_play")

    %Board{grid: grid} = Runner.get_board()
    players = Runner.get_players()

    {
      :ok,
      socket
      |> assign(:grid, grid)
      |> assign(:players, players)
    }
  end

  def handle_info({:move, %Board{grid: grid}}, socket) do
    {
      :noreply,
      socket
      |> assign(:grid, grid)
    }
  end

  def handle_info({:attack, players}, socket) do
    {
      :noreply,
      socket
      |> assign(:players, players)
    }
  end
end
