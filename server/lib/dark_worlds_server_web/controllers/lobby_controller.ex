defmodule DarkWorldsServerWeb.LobbyController do
  use DarkWorldsServerWeb, :controller

  alias DarkWorldsServer.Matchmaking
  alias DarkWorldsServer.Communication

  def new(conn, _params) do
    matchmaking_session_pid = Matchmaking.create_session()

    headers = Enum.into(conn.req_headers, %{})
    session_id = Communication.pid_to_external_id(matchmaking_session_pid)

    if headers["content-type"] == "application/json" do
      json(conn, %{session_id: session_id})
    else
      redirect(conn, to: "/matchmaking/#{session_id}")
    end
  end
end
