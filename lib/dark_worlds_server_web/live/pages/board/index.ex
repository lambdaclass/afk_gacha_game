defmodule DarkWorldsServerWeb.BoardLive.Index do
  use DarkWorldsServerWeb, :live_view

  alias DarkWorldsServer.Communication
  alias DarkWorldsServer.Engine.{Runner, Board}

  def mount(%{"game_id" => game_id}, _session, socket) do
    if connected?(socket) do
      DarkWorldsServer.PubSub
      |> Phoenix.PubSub.subscribe("game_play_#{game_id}")
    end

    runner_pid = Communication.external_id_to_pid(game_id)
    %Board{grid: grid} = Runner.get_board(runner_pid)
    players = Runner.get_players(runner_pid)

    {
      :ok,
      socket
      |> assign(
        runner_pid: runner_pid,
        grid: grid,
        players: players,
        game_id: game_id,
        pings: %{},
        count: 0
      )
    }
  end

  def mount(_params, _session, socket) do
    {:ok, socket}
  end

  def handle_params(%{"game_id" => game_id}, _url, socket) do
    runner_pid = Communication.external_id_to_pid(game_id)
    %Board{grid: grid} = Runner.get_board(runner_pid)
    players = Runner.get_players(runner_pid)

    {:noreply, socket |> assign(runner_pid: runner_pid, grid: grid, players: players)}
  end

  def handle_params(_params, _url, socket) do
    {:noreply, socket}
  end

  def handle_info({:move, %{game: game}}, socket) do
    {
      :noreply,
      socket
      |> assign(:grid, game.board.grid)
      |> assign(:count, socket.assigns.count + 1)
    }
  end

  def handle_info({:attack, %{game: game}}, socket) do
    {
      :noreply,
      socket
      |> assign(:players, game.players)
      |> assign(:grid, game.board.grid)
    }
  end

  def handle_info({:game_finished, %{game: game}}, socket) do
    {
      :noreply,
      socket
      |> assign(:players, game.players)
    }
  end

  def handle_info({:update_ping, player, ping}, socket) do
    pings = socket.assigns.pings

    {
      :noreply,
      socket
      |> assign(:pings, Map.put(pings, player, ping))
    }
  end
end
