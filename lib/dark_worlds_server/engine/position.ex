defmodule DarkWorldsServer.Engine.Position do
  @enforce_keys [:x, :y]
  @derive Jason.Encoder
  defstruct [:x, :y]
end
