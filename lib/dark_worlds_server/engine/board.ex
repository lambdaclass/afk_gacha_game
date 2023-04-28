defmodule DarkWorldsServer.Engine.Board do
  use DarkWorldsServer.Communication.Encoder

  @enforce_keys [:grid]
  defstruct [:grid]
end
