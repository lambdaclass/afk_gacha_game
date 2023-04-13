defmodule DarkWorldsServerWeb.PlayWebSocket do
  @moduledoc """
  Play Websocket handler that parses msgs to be send to the runner genserver
  """
  alias DarkWorldsServer.Engine.{Action, Runner}

  @behaviour :cowboy_websocket

  def init(req, opts), do: {:cowboy_websocket, req, opts}

  def websocket_init(state) do
    {:ok, state}
  end

  def websocket_handle({:text, message}, state) do
    case Action.from_json(message) do
      {:ok, action} ->
        Runner.play(action)
        {:ok, state}

      {:error, _error} ->
        {:reply, {:text, "ERROR: Could not decode json"}, state}
    end
  end

  def websocket_info(info, state), do: {:reply, {:text, info}, state}
end
