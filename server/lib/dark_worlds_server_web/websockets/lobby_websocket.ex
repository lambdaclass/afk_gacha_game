defmodule DarkWorldsServerWeb.LobbyWebsocket do
  alias DarkWorldsServer.Matchmaking
  alias DarkWorldsServer.Communication

  @behaviour :cowboy_websocket

  def init(req, _opts) do
    lobby_id = :cowboy_req.binding(:lobby_id, req)
    {:cowboy_websocket, req, %{lobby_id: lobby_id}}
  end

  def websocket_init(%{lobby_id: :undefined}) do
    {:stop, %{}}
  end

  def websocket_init(%{lobby_id: lobby_id}) do
    matchmaking_session_id = Matchmaking.create_session()

    {:reply, {:text, "CONNECTED_TO: #{Communication.pid_to_external_id(matchmaking_session_id)}"},
     %{runner_pid: matchmaking_session_id}}
  end

  def websocket_handle(message, action) do
    IO.inspect(message, label: :mensaje)
    IO.inspect(action, label: :action)
  end

  def websocket_info(message, state) do
    IO.inspect(message, label: :mensaje)
    IO.inspect(state, label: :state)
  end
end
