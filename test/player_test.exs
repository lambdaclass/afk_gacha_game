defmodule DarkWorldsServer.PlayerTest do
  use DarkWorldsServerWeb.ConnCase, async: true

  alias DarkWorldsServer.WsClient
  alias Plug.Conn

  describe "Move the player around" do
    test "Move right", %{conn: conn} do
      session_id = create_session(conn)
      {:ok, _ws_pid} = ws_connect(session_id)

      first_player_before_moving = WsClient.get_players(session_id) |> List.first()
      WsClient.move(1, :up)
      :timer.sleep(1_000)
      first_player_after_moving = WsClient.get_players(session_id) |> List.first()

      if first_player_before_moving.position.x == 0 do
        assert first_player_after_moving.position.x == first_player_before_moving.position.x
      else
        assert first_player_after_moving.position.x == first_player_before_moving.position.x - 1
      end
    end
  end

  defp create_session(conn) do
    conn = Conn.put_req_header(conn, "content-type", "application/json")
    new_session = get(conn, ~p"/new_session", %{})
    new_session = json_response(new_session, 200)
    Map.get(new_session, "session_id")
  end

  defp ws_connect(session_id) do
    WsClient.start_link("ws://localhost:4002/play/#{session_id}")
  end
end
