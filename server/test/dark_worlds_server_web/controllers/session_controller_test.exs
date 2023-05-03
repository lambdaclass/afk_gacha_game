defmodule DarkWorldsServerWeb.SessionControllerTest do
  use DarkWorldsServerWeb.ConnCase, async: true

  alias Plug.Conn

  describe "GET /new_session" do
    test "Session is created succesfully", %{conn: conn} do
      conn = Conn.put_req_header(conn, "content-type", "application/json")
      conn = get(conn, ~p"/new_session", %{})

      response = json_response(conn, 200)
      assert Map.has_key?(response, "session_id")
    end

    @tag :board_list
    test "Board lists all active game sessions", %{conn: conn} do
      conn = Conn.put_req_header(conn, "content-type", "application/json")
      new_session = get(conn, ~p"/new_session", %{})
      new_session = json_response(new_session, 200)
      session_id = Map.get(new_session, "session_id")
      board = get(conn, ~p"/board", %{})
      board = board.resp_body
      assert board =~ session_id
    end
  end
end
