defmodule DarkWorldsServerWeb.LobbyWebsocket do
  alias DarkWorldsServer.Matchmaking
  alias DarkWorldsServer.Matchmaking.MatchingSession
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

    matchmaking_session_pid = Communication.external_id_to_pid(lobby_id)
    players = MatchingSession.list_players(matchmaking_session_pid)
    player_id = (Enum.count(players) + 1)
    MatchingSession.add_player(player_id, matchmaking_session_pid)

    {:reply, {:text, "CONNECTED_TO: #{lobby_id} YOU'RE PLAYER #{player_id}"},
     %{runner_pid: matchmaking_session_pid}}
  end
end
