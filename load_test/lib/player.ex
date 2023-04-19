defmodule LoadTest.Player do
  use WebSockex
  require Logger
  use Tesla

  @server_host "localhost:4000"

  def move(player, :up), do: _move(player, "up")
  def move(player, :down), do: _move(player, "down")
  def move(player, :left), do: _move(player, "left")
  def move(player, :right), do: _move(player, "right")

  defp _move(player, direction) do
    %{"player" => player, "action" => "move", "value" => direction}
    |> send_command()
  end

  def start_link({player_number, session_id}) do
    WebSockex.start_link("ws://#{@server_host}/play/#{session_id}", __MODULE__, %{
      player_number: player_number,
      session_id: session_id
    })
  end

  def handle_frame({type, msg}, state) do
    Logger.info("Received Message: #{inspect(msg)}")
    {:ok, state}
  end

  def handle_cast({:send, {type, msg} = frame}, state) do
    Logger.info("Sending frame with payload: #{msg}")
    {:reply, frame, state}
  end

  def handle_info(:play, state) do
    direction = Enum.random([:up, :down, :left, :right])
    move(state.player_number, direction)
    Process.send_after(self(), :play, 100, [])
    {:ok, state}
  end

  defp send_command(command) do
    WebSockex.cast(self(), {:send, {:text, Jason.encode!(command)}})
  end
end
