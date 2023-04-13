defmodule DarkWorldsServer.Engine do
  @moduledoc """
  Game Engine Supervisor
  """
  use Supervisor

  alias DarkWorldsServer.Engine.Runner

  def start_link(args) do
    Supervisor.start_link(__MODULE__, args, name: __MODULE__)
  end

  @impl true
  def init(_opts) do
    children = [
      Runner
    ]

    Supervisor.init(children, strategy: :one_for_one)
  end
end
