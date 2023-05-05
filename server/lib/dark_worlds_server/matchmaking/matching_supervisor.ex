defmodule DarkWorldsServer.Matchmaking.MatchingSupervisor do
  @moduledoc """
  Supervisor for matchmaking sessions
  """
  use DynamicSupervisor
  alias DarkWorldsServer.Matchmaking.MatchingSession

  def start_link(args) do
    DynamicSupervisor.start_link(__MODULE__, args, name: __MODULE__)
  end

  def start_child() do
    DynamicSupervisor.start_child(__MODULE__, MatchingSession)
  end

  def children_pids() do
    DynamicSupervisor.which_children(__MODULE__)
    |> Enum.map(fn {_, pid, _, _} -> pid end)
  end

  @impl DynamicSupervisor
  def init(_opts) do
    DynamicSupervisor.init(strategy: :one_for_one)
  end
end
