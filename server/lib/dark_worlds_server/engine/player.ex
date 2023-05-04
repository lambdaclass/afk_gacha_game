defmodule DarkWorldsServer.Engine.Player do
  use DarkWorldsServer.Communication.Encoder

  @enforce_keys [:id, :health, :position]
  defstruct [:id, :health, :position]
end
