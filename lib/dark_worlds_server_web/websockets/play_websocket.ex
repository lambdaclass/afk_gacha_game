
defmodule DarkWorldsServerWeb.PlayWebSocket do
  @moduledoc """
  Play Websocket handler that parses msgs to be send to the runner genserver
  """
  import Jason, only: [decode: 1]

  @behaviour :cowboy_websocket

  @impl :cowboy_websocket
  def init(req, opts), do: {:cowboy_websocket, req, opts}

  @impl :cowboy_websocket
  def websocket_init(state), do: {[], state}

  @impl :cowboy_websocket
  def websocket_handle(frame, state)

  def websocket_handle(:ping, state) do
    {[:pong], state}
  end

  def websocket_handle({:text, message}, state) do
    {:ok, data} = decode(message)

    {[{:text, "OK"}], state}
  end

  @impl :cowboy_websocket
  def websocket_info(info, state)

  def websocket_info(_info, state), do: {[], state}
end
