defmodule DarkWorldsServerWeb.SessionController do
  use DarkWorldsServerWeb, :controller
  alias DarkWorldsServer.Engine
  alias DarkWorldsServer.Engine.Runner

  def new(conn, _params) do
    {:ok, runner_pid} = Engine.start_child()

    headers = Enum.into(conn.req_headers, %{})
    session_id = Runner.encode_pid(runner_pid)

    if headers["content-type"] == "application/json" do
      json(conn, %{session_id: session_id})
    else
      redirect(conn, to: "/board/#{session_id}")
    end
  end
end
