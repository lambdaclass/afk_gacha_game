defmodule DarkWorldsServerWeb.LobbyController do
  use DarkWorldsServerWeb, :controller

  alias DarkWorldsServer.Matchmaking
  alias DarkWorldsServer.Matchmaking.MatchingSupervisor
  alias DarkWorldsServer.Communication

  def new(conn, _params) do
    matchmaking_session_pid = Matchmaking.create_session()

    headers = Enum.into(conn.req_headers, %{})
    lobby_id = Communication.pid_to_external_id(matchmaking_session_pid)

    if headers["content-type"] == "application/json" do
      json(conn, %{lobby_id: lobby_id})
    else
      redirect(conn, to: "/matchmaking/#{lobby_id}")
    end
  end

  def current_lobbies(conn, _params) do
    headers = Enum.into(conn.req_headers, %{}) |> IO.inspect()
    matchmaking_pids = MatchingSupervisor.children_pids()
    lobbies = Enum.map(matchmaking_pids, fn pid -> Communication.pid_to_external_id(pid) end)

    if headers["content-type"] == "application/json" do
      json(conn, %{lobbies: lobbies})
    else
      redirect(conn, to: "/matchmaking")
    end
  end
end
