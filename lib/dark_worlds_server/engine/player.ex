defmodule DarkWorldsServer.Engine.Player do
  @enforce_keys [:id, :health, :position]
  @derive Jason.Encoder
  defstruct [:id, :health, :position]
end
