defmodule DarkWorldsServer.Engine.Player do
  use DarkWorldsServer.Communication.Encoder

  @enforce_keys [:id, :health, :position, :action]
  defstruct [:id, :health, :position, :action]
end
