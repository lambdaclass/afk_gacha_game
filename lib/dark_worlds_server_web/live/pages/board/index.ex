defmodule DarkWorldsServerWeb.BoardLive.Index do
  use DarkWorldsServerWeb, :live_view

  alias DarkWorldsServer.Engine.{Runner, Board}

  def mount(%{"game_id" => encoded_game_id}, _session, socket) do
    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.subscribe("game_play_#{encoded_game_id}")

    runner_pid = Base.decode64!(encoded_game_id) |> :erlang.binary_to_term([:safe])
    %Board{grid: grid} = Runner.get_board(runner_pid)
    players = Runner.get_players(runner_pid)

    {
      :ok,
      socket
      |> assign(runner_pid: runner_pid, grid: grid, players: players)
    }
  end

  def mount(_params, _session, socket) do
    {:ok, socket}
  end

  def handle_params(%{"game_id" => encoded_game_id}, _url, socket) do
    DarkWorldsServer.PubSub
    |> Phoenix.PubSub.subscribe("game_play_#{encoded_game_id}")

    runner_pid = Base.decode64!(encoded_game_id) |> :erlang.binary_to_term([:safe])
    %Board{grid: grid} = Runner.get_board(runner_pid)
    players = Runner.get_players(runner_pid)

    {:noreply, socket |> assign(runner_pid: runner_pid, grid: grid, players: players)}
  end

  def handle_params(_params, _url, socket) do
    {:noreply, socket}
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

  def handle_info({:game_finished, game}, socket) do
    {
      :noreply,
      socket
      |> assign(:players, game.players)
    }
  end
end
