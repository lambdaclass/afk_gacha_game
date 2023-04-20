defmodule DarkWorldsServer.Matchmaking do
  @moduledoc """
  The Matchmaking context
  """

  alias DarkWorldsServer.Matchmaking.MatchingSession
  alias DarkWorldsServer.Matchmaking.MatchingSupervisor

  def create_session() do
    {:ok, pid} = MatchingSupervisor.start_child()
    encode_pid(pid)
  end

  def add_player(player_id, session_id) do
    session_pid = decode_pid(session_id)
    MatchingSession.add_player(player_id, session_pid)
  end

  def remove_player(player_id, session_id) do
    session_pid = decode_pid(session_id)
    MatchingSession.remove_player(player_id, session_pid)
  end

  def fetch_sessions() do
    Enum.map(MatchingSupervisor.children_pids(), fn pid -> encode_pid(pid) end)
  end

  def session_topic(session_id) do
    "matching_session_#{session_id}"
  end

  defp encode_pid(pid) do
    pid |> :erlang.term_to_binary() |> Base.encode64()
  end

  defp decode_pid(encoded) do
    encoded |> Base.decode64!() |> :erlang.binary_to_term([:safe])
  end
end
