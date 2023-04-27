defmodule DarkWorldsServer.PlayerTest do
  use DarkWorldsServerWeb.ConnCase, async: true

  alias DarkWorldsServer.WsClient
  alias Plug.Conn

  describe "Move the player around" do
    @tag :move_up
    test "Move up", %{conn: conn} do
      session_id = create_session(conn)
      {:ok, _ws_pid} = ws_connect(session_id)
      board = WsClient.get_board(session_id)

      first_player_before_moving = WsClient.get_players(session_id) |> List.first()
      WsClient.move(1, :up)
      :timer.sleep(1_000)
      first_player_after_moving = WsClient.get_players(session_id) |> List.first()

      walls = get_wall_coordinates(board.grid)
      IO.inspect(board.grid)
      IO.inspect(walls)
      IO.inspect(first_player_before_moving.position)
      IO.inspect(first_player_after_moving.position)

      if first_player_before_moving.position.x == 0 do
        assert first_player_after_moving.position.x == first_player_before_moving.position.x
        else # if there's a wall above, assert that player didn't move up
        if {first_player_before_moving.position.x - 1, first_player_before_moving.position.y} in walls do
          assert first_player_after_moving.position.x == first_player_before_moving.position.x
        else
          assert first_player_after_moving.position.x == first_player_before_moving.position.x - 1
        end
      end
    end

    @tag :move_down
    test "Move down", %{conn: conn} do
      session_id = create_session(conn)
      {:ok, _ws_pid} = ws_connect(session_id)
      board = WsClient.get_board(session_id)

      first_player_before_moving = WsClient.get_players(session_id) |> List.first()
      WsClient.move(1, :down)
      :timer.sleep(1_000)
      first_player_after_moving = WsClient.get_players(session_id) |> List.first()

      walls = get_wall_coordinates(board.grid)

      if first_player_before_moving.position.x == board.height - 1 do
        assert first_player_after_moving.position.x == first_player_before_moving.position.x
      else # if there's a wall below, assert that player didn't move down
        if {first_player_before_moving.position.x + 1, first_player_before_moving.position.y} in walls do
          assert first_player_after_moving.position.x == first_player_before_moving.position.x
        else
          assert first_player_after_moving.position.x == first_player_before_moving.position.x + 1
        end
      end
    end
  end

  def get_wall_coordinates(board_matrix) do
    board_matrix
    |> Enum.with_index()
    |> Enum.flat_map(fn {row, x} ->
      row
      |> Enum.with_index()
      |> Enum.filter(fn {cell, _} -> cell == :wall end)
      |> Enum.map(fn {_, y} -> {x, y} end)
    end)
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
