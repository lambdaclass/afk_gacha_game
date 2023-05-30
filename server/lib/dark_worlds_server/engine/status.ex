defmodule DarkWorldsServer.Engine.Status do
  use DarkWorldsServer.Communication.Encoder

  @enforce_keys [:id, :health, :position, :last_melee_attack, :status]
  defstruct [:id, :health, :position, :last_melee_attack, :status]
end
