defmodule DarkWorldsServerWeb.PlayWebSocket do
  @moduledoc """
  Play Websocket handler that parses msgs to be send to the runner genserver
  """
  alias DarkWorldsServer.Engine.{ActionRaw, ActionOk, Runner}
  alias DarkWorldsServer.Engine

  @behaviour :cowboy_websocket

  def init(req, _opts) do
    game_id = :cowboy_req.binding(:game_id, req)
    {:cowboy_websocket, req, %{game_id: game_id}}
  end

  def websocket_init(%{game_id: :undefined}) do
    {:stop, %{}}
  end

  def websocket_init(%{game_id: game_id}) do
    runner_pid = game_id |> URI.decode_www_form() |> Runner.game_id_to_pid()

    state = %{runner_pid: runner_pid}

    if runner_pid in Engine.list_runners_pids() do
      {:reply, {:text, "CONNECTED_TO: #{runner_pid |> Runner.pid_to_game_id()}"}, state}
    else
      {:stop, state}
    end
  end

  def websocket_handle({:text, message}, state) do
    case ActionRaw.from_json(message) do
      {:ok, action} ->
        IO.inspect(action)

        case ActionOk.from_action_raw(action) do
          {:ok, action} ->
            Runner.play(state[:runner_pid], action)
            {:reply, {:text, "OK"}, state}

          {:error, msg} ->
            {:reply, {:text, "ERROR: #{msg}"}, state}
        end

      {:error, _error} ->
        {:reply, {:text, "ERROR: Invalid json"}, state}
    end
  end

  def websocket_info(info, state), do: {:reply, {:text, info}, state}
end
