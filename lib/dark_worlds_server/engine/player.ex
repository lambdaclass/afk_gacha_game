defmodule DarkWorldsServer.Engine.Player do
  @enforce_keys [:number, :health, :position]
  defstruct [:number, :health, :position]

  def new(number, health, {y, x} = position \\ {0, 0})
      when is_integer(number) and is_integer(health) and is_integer(y) and is_integer(x) do
    %__MODULE__{number: number, health: health, position: position}
  end
end
