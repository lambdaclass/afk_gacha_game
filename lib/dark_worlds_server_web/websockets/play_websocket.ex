defmodule DarkWorldsServerWeb.PlayWebSocket do
  @moduledoc """
  Play Websocket handler that parses msgs to be send to the runner genserver
  """
  alias DarkWorldsServer.Engine
  alias DarkWorldsServer.Engine.{ActionRaw, ActionOk, Runner}

  @behaviour :cowboy_websocket

  def init(req, _opts) do
    game_id = :cowboy_req.header("game_id", req)
    IO.inspect(DynamicSupervisor.which_children(DarkWorldsServer.Engine))
    {:cowboy_websocket, req, %{game_id: game_id}}
  end

  def websocket_init(%{game_id: :undefined}) do
    {:ok, runner_pid} = Engine.start_child()

    {:reply, {:text, "CONNECTED_NEW: #{runner_pid |> Runner.pid_to_game_id()}"},
     %{runner_pid: runner_pid}}
  end

  def websocket_init(%{game_id: game_id}) do
    runner_pid = game_id |> Runner.game_id_to_pid()

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
