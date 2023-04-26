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
      conn = Conn.put_req_header(conn, "content-type", "application/http")
      new_session = get(conn, ~p"/new_session", %{})
      new_session = new_session.resp_body
      new_session = String.slice(new_session, 42..-32)
      board = get(conn, ~p"/board", %{})
      board = board.resp_body
      String.contains? board, new_session
    end
  end
end
