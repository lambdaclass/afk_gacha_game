defmodule DarkWorldsServer.Engine.ActionOk do
  alias DarkWorldsServer.Engine.ActionRaw

  @enforce_keys [:action, :value]
  @derive Jason.Encoder
  defstruct [:action, :value]

  def from_action_raw(%ActionRaw{
        action: {:ok, action},
        value: {:ok, value}
      }) do
    {:ok, %__MODULE__{action: action, value: value}}
  end

  def from_action_raw(_) do
    {:error, "Invalid message"}
  end
end
