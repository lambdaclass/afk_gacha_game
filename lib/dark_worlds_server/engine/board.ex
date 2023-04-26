defmodule DarkWorldsServer.Engine.Board do
  @enforce_keys [:grid]
  @derive Jason.Encoder
  defstruct [:grid]
end
