defmodule DarkWorldsServer.Engine.Player do
  @enforce_keys [:number, :health]
  defstruct [:number, :health]

  def new(number, health) when is_integer(number) and is_integer(health) do
    %__MODULE__{number: number, health: health}
  end
end
