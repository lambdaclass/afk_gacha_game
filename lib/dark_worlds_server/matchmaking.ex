defmodule DarkWorldsServer.Matchmaking do
  @moduledoc """
  The Matchmaking context
  """

  alias DarkWorldsServer.Communication
  alias DarkWorldsServer.Matchmaking.MatchingSession
  alias DarkWorldsServer.Matchmaking.MatchingSupervisor

  def create_session() do
    {:ok, pid} = MatchingSupervisor.start_child()
    Communication.pid_to_external_id(pid)
  end

  def add_player(player_id, session_id) do
    session_pid = Communication.external_id_to_pid(session_id)
    MatchingSession.add_player(player_id, session_pid)
  end

  def remove_player(player_id, session_id) do
    session_pid = Communication.external_id_to_pid(session_id)
    MatchingSession.remove_player(player_id, session_pid)
  end

  def fetch_sessions() do
    Enum.map(MatchingSupervisor.children_pids(), fn pid -> Communication.pid_to_external_id(pid) end)
  end

  def session_topic(session_id) do
    "matching_session_#{session_id}"
  end

  def start_game(session_id) do
    session_pid = Communication.external_id_to_pid(session_id)
    MatchingSession.start_game(session_pid)
  end
end
