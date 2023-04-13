defmodule DarkWorldsServerWeb.PlayWebSocket do
  @moduledoc """
  Play Websocket handler that parses msgs to be send to the runner genserver
  """
  alias DarkWorldsServer.Engine.{ActionRaw, ActionOk, Runner}

  @behaviour :cowboy_websocket

  def init(req, opts), do: {:cowboy_websocket, req, opts}

  def websocket_init(state) do
    {:ok, state}
  end

  def websocket_handle({:text, message}, state) do
    case ActionRaw.from_json(message) do
      {:ok, action} ->
        IO.inspect(action)
        case ActionOk.from_action_raw(action) do
          {:ok, action} ->
            Runner.play(action)
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
