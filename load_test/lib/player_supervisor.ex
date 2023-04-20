defmodule LoadTest.PlayerSupervisor do
  @moduledoc """
  Player Supervisor
  """
  use DynamicSupervisor
  use Tesla
  plug(Tesla.Middleware.Headers, [{"content-type", "application/json"}])

  alias LoadTest.Player

  @server_host "localhost:4000"

  def start_link(args) do
    DynamicSupervisor.start_link(__MODULE__, args, name: __MODULE__)
  end

  def spawn_player(player_number, session_id) do
    DynamicSupervisor.start_child(__MODULE__, {Player, {player_number, session_id}})
  end

  @impl true
  def init(_opts) do
    DynamicSupervisor.init(strategy: :one_for_one)
  end

  # Spawns a game lobby, populates it with players and starts moving them randomly
  def spawn_session() do
    {:ok, response} = get("http://#{@server_host}/new_session")
    %{"session_id" => session_id} = Jason.decode!(response.body)

    for i <- 1..2 do
      {:ok, pid} = spawn_player(i, session_id)
      Process.send(pid, :play, [])
    end
  end

  def children_pids() do
    DynamicSupervisor.which_children(__MODULE__)
    |> Enum.map(fn {_, pid, _, _} -> pid end)
  end
end
