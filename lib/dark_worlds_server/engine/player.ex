defmodule DarkWorldsServer.Engine.Player do
  @enforce_keys [:number, :health, :position]
  defstruct [:number, :health, :position]
end
