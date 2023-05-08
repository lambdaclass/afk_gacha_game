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
  end
end
