defmodule DarkWorldsServerWeb.MatchmakingLive.Index do
  use DarkWorldsServerWeb, :live_view
  alias DarkWorldsServer.Matchmaking

  def mount(_params, _session, socket) do
    case connected?(socket) do
      false ->
        {:ok, socket}

      true ->
        Process.send_after(self(), :fetch_sessions, 10_000)
        {:ok, assign(socket, :game_session_ids, Matchmaking.fetch_sessions())}
    end
  end

  def handle_event("create_session", _params, socket) do
    session_id = Matchmaking.create_session()
    {:noreply, push_navigate(socket, to: ~p"/matchmaking/#{session_id}")}
  end

  def handle_info(:fetch_sessions, socket) do
    Process.send_after(self(), :fetch_sessions, 10_000)
    {:noreply, assign(socket, :game_session_ids, Matchmaking.fetch_sessions())}
  end
end
